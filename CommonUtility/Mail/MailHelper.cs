using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utility.CommonUtility.Extension;

namespace Utility.CommonUtility.Mail
{
    /// <summary>
    /// Helper for send mail 
    /// </summary>
    public sealed class MailHelper
    {

        #region Properties

        public Action SendCompletedHandler;

        private bool _mailSendEnabled = true;             //if can send mail or not
        private bool _mailAnonymousAccess = false;        //if anonymous access
        private int _mailTimeout = 60000;                 //timeout of send mail(ms)
        private int _mailPort = 25;                       //smtp port
        private string _mailHost = string.Empty;          //SMTP server address
        private string _mailAccount = string.Empty;       //SMTP account
        private string _mailPwd = string.Empty;           //SMTP password
        private string _mailFrom = string.Empty;          //sender address
        private string[] _mailTo = null;                  //to addresss
        private string[] _mailReplyTo = null;             //reply to addresss

        #endregion

        #region Contructors

        public MailHelper()
        {
            _mailSendEnabled = ConfigurationManagerExtension.GetBoolValue("MAIL_SEND_ENABLED");
            _mailAnonymousAccess = ConfigurationManagerExtension.GetBoolValue("MAIL_ANONYMOUS_ACCESS");
            _mailTimeout = ConfigurationManagerExtension.GetIntValue("MAIL_TIMEOUT") * 1000;
            _mailHost = ConfigurationManagerExtension.GetStringValue("MAIL_HOST");
            _mailAccount = ConfigurationManagerExtension.GetStringValue("MAIL_ACCOUNT");
            _mailPwd = ConfigurationManagerExtension.GetStringValue("MAIL_PASSWORD");
            _mailFrom = ConfigurationManagerExtension.GetStringValue("MAIL_FROM");
            _mailTo = string.IsNullOrEmpty(ConfigurationManagerExtension.GetStringValue("MAIL_TO")) ? null : ConfigurationManagerExtension.GetStringValue("MAIL_TO").Split(new char[] { ',' });
            _mailPort = ConfigurationManagerExtension.GetIntValue("MAIL_PORT");
            _mailReplyTo = string.IsNullOrEmpty(ConfigurationManagerExtension.GetStringValue("MAIL_REPLYTO")) ? null : ConfigurationManagerExtension.GetStringValue("MAIL_REPLYTO").Split(new char[] { ',' });
        }

        public MailHelper(string host, string account, string password, string from, string[] to, int port)
        {
            this._mailHost = host;
            this._mailAccount = account;
            this._mailPwd = password;
            this._mailFrom = from;
            this._mailTo = to;
            this._mailPort = port;
        }
        #endregion //Contructors

        #region Send Mail

        /// <summary>
        /// send mail
        /// </summary>
        /// <param name="to">receivers mail-addredss</param>
        /// <param name="subject">email subject</param>
        /// <param name="content">email content</param>
        /// <returns>status of send mail</returns>
        public bool SendMail(string[] to, string subject, string content)
        {
            return SendMail(to, subject, content, null, true);
        }

        /// <summary>
        ///  send mail
        /// </summary>
        /// <param name="to">receiver mail-address</param>
        /// <param name="title">email subject</param>
        /// <param name="content">email content</param>
        /// <returns>status of send mail</returns>
        public bool SendMail(string to, string title, string content)
        {
            return SendMail(new string[] { to }, title, content); ;
        }

        /// <summary>
        /// send mail
        /// </summary>
        /// <param name="title">mail subject</param>
        /// <param name="content">mail content</param>
        /// <returns>status of send mail</returns>
        public bool SendMail(string title, string content, bool isAsync)
        {
            return SendMail(this._mailTo, title, content, null, isAsync);
        }

        /// <summary>
        /// send mail
        /// </summary>
        /// <param name="title">mail subject</param>
        /// <param name="content">mail content</param>
        /// <param name="attachments">email attachments</param>
        /// <returns>status of send mail</returns>
        public bool SendMail(string title, string content, string[] attachments, bool isAsync)
        {
            return SendMail(this._mailTo, title, content, attachments, isAsync);
        }

        /// <summary>
        /// send mail to any people
        /// </summary>
        /// <param name="mailTos">receivers mail-addredss</param>
        /// <param name="subject">email subject</param>
        /// <param name="content">email content</param>
        /// <param name="attachments">email attachments</param>
        /// <param name="isAsync">is ascync or not</param>
        /// <returns>status of send mail</returns>
        public bool SendMail(string[] mailTos, string subject, string content, string[] attachments, bool isAsync)
        {
            if (!_mailSendEnabled)
            {
                throw new Exception("Mail Send Function is disable!");
            }

            using (SmtpClient _smtpClient = new SmtpClient())
            {
                //create smtp client with account and password
                _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//send the mail style
                _smtpClient.Host = _mailHost; //smtp server
                _smtpClient.Port = _mailPort;

                if (_mailAnonymousAccess)
                {
                    //do something
                }
                else
                {
                    _smtpClient.EnableSsl = true;
                    _smtpClient.Credentials = new System.Net.NetworkCredential(_mailAccount, _mailPwd);//user name & password
                }

                //create message for mail
                MailMessage _mailMessage = new MailMessage();
                _mailMessage.From = new MailAddress(_mailFrom);

                if (mailTos != null)
                {
                    foreach (string to in mailTos)
                    {
                        _mailMessage.To.Add(to);
                    }
                }

                _mailMessage.Subject = subject;
                _mailMessage.Body = content;
                _mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                _mailMessage.IsBodyHtml = true;
                _mailMessage.Priority = MailPriority.Normal;
                _mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                if (_mailReplyTo != null)
                {
                    foreach (var item in _mailReplyTo)
                    {
                        _mailMessage.ReplyToList.Add(item);
                    }
                }

                if (attachments != null)
                {
                    foreach (var item in attachments)
                    {
                        _mailMessage.Attachments.Add(new Attachment(item));
                    }
                }

                //send mail
                try
                {
                    //send by async
                    if (isAsync)
                    {
                        _smtpClient.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
                        string userState = null;
                        _smtpClient.SendAsync(_mailMessage, userState);
                    }
                    else
                    {
                        // send by sync
                        _smtpClient.Timeout = _mailTimeout;
                        _smtpClient.Send(_mailMessage);
                    }
                }
                catch (Exception e)
                {
                    //网络原因或者发件邮件配置错误导致发送失败
                    throw e;
                }
                finally
                {
                    //dispose attachment
                    foreach (Attachment attachment in _mailMessage.Attachments)
                    {
                        attachment.Dispose();
                    }
                    _mailMessage.Attachments.Dispose();
                }
            }

            return true;
        }

        /// <summary>
        /// callback of send completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            // String token = (string)e.UserState;

            if (SendCompletedHandler != null)
                SendCompletedHandler();
        }

        #endregion Send Mail

        #region Verify Mail

        /// <summary>
        /// use regex to verify email address
        /// </summary>
        /// <param name="emailAddress">Email address</param>
        /// <returns></returns>
        public bool VerifyWithRegex(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
                return false;

            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

            Regex re = new Regex(strRegex);
            if (re.IsMatch(emailAddress))
                return true;
            else
                return false;
        }

        /// <summary>
        /// use telnet to connect email address
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public bool TelnetEmailServer(string emailAddress)
        {
            bool result = false;

            // get hostname
            var arrHost = emailAddress.Split('@');
            var hostName = arrHost[1];

            // get dnsname
            IList<string> dnsNames = GetDNSNames(hostName);
            if (dnsNames == null || dnsNames.Count == 0)
            {
                return result;
            }

            //telnet email server
            string smtpHostName = dnsNames[0];
            int smtpHost = 25;

            TcpClient tcpClient = new TcpClient(smtpHostName, smtpHost);

            string CRLF = "\r\n";
            byte[] dataBuffer;
            string responseString;

            NetworkStream netStream = tcpClient.GetStream();
            StreamReader reader = new StreamReader(netStream);
            responseString = reader.ReadLine();

            // Perform HELO to SMTP Server and get Response
            dataBuffer = Encoding.ASCII.GetBytes("HELO server.com" + CRLF);
            netStream.Write(dataBuffer, 0, dataBuffer.Length);
            responseString = reader.ReadLine();

            // Perform MAIL FROM to a special email and get Response
            dataBuffer = Encoding.ASCII.GetBytes("MAIL FROM:<" + emailAddress + ">" + CRLF);
            netStream.Write(dataBuffer, 0, dataBuffer.Length);
            responseString = reader.ReadLine();

            // Read Response of the RCPT TO Message to know from email server if the email exist or not 
            dataBuffer = Encoding.ASCII.GetBytes("RCPT TO:<" + emailAddress + ">" + CRLF);
            netStream.Write(dataBuffer, 0, dataBuffer.Length);
            responseString = reader.ReadLine();

            //check if the response string include 550 
            if (!responseString.Contains("250"))//responseString.Contains("550")
            {
                // Mail Address Does not Exist

                result = false;
            }
            else
            {
                result = true;
            }

            // QUITE Connection
            dataBuffer = Encoding.ASCII.GetBytes("QUITE" + CRLF);
            netStream.Write(dataBuffer, 0, dataBuffer.Length);

            // close tcp connection
            tcpClient.Close();

            return result;
        }

        /// <summary>
        /// use nslookup to get hostname
        /// notes:the system must include nslookup
        /// </summary>
        /// <param name="hostname">email's hostname</param>
        /// <returns></returns>
        public IList<string> GetDNSNames(string hostname)
        {
            IList<string> emailExchanger = new List<string>();

            string output;

            //create nslookup process & run
            var startInfo = new ProcessStartInfo("nslookup");
            startInfo.Arguments = string.Format("-type=MX {0}", hostname);
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            using (var cmd = Process.Start(startInfo))
            {
                output = cmd.StandardOutput.ReadToEnd();
            }

            //use regex to filter output string
            string pattern = @"mail exchanger =\s*([\w\-\=\.]*)";
            MatchCollection matches = Regex.Matches(output, pattern, RegexOptions.IgnoreCase);
            foreach (Match match in matches)
            {
                if (match.Success)
                    emailExchanger.Add(match.Groups[1].Value);
            }

            return emailExchanger;
        }

        #endregion Verify Mail
    }
}
