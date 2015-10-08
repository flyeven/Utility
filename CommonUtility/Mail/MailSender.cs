﻿using Utility.CommonUtility.Xml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.CommonUtility.Mail
{
    public class MailSender
    {
        private string _moduleName = "Email";
        public string ModuleName
        {
            get { return _moduleName; }
            set { _moduleName = value; }
        }

        private MailHelper _mailHelper = new MailHelper();

        /// <summary>
        /// send email
        /// </summary>
        /// <param name="subject">the subject of email</param>
        /// <param name="templateName">the template name of the mail content</param>
        /// <param name="paraTable">email model</param>
        public void SendMail(string subject, string templateName, Hashtable paraTable)
        {
            SendMail(subject, null, templateName, paraTable, true);
        }

        /// <summary>
        /// send email
        /// </summary>
        /// <param name="subject">the subject of email</param>
        /// <param name="templateName">the template name of the mail content</param>
        /// <param name="isAsync">if async or not</param>
        /// <param name="paraTable">email model</param>
        public void SendMail(string subject, string templateName, Hashtable paraTable, bool isAsync)
        {
            SendMail(subject, null, templateName, paraTable, isAsync);
        }

        /// <summary>
        /// send email
        /// </summary>
        /// <param name="subject">the subject of email</param>
        /// <param name="attachments">the attachments of email</param>
        /// <param name="templateName">the template name of the mail content</param>
        /// <param name="paraTable">email model</param>
        /// <param name="isAsync">if async or not</param>
        public bool SendMail(string subject, string[] attachments, string templateName, Hashtable paraTable, bool isAsync)
        {
            string template = GetTemplate(templateName, paraTable);
            return _mailHelper.SendMail(subject, template, attachments, isAsync);
        }

        /// <summary>
        /// send mail to any people
        /// </summary>
        /// <param name="to">receivers mail-addredss</param>
        /// <param name="subject">email subject</param>
        /// <param name="templateName">the template name of the mail content</param>
        /// <param name="paraTable">email model</param>
        /// <param name="attachments">email attachments</param>
        /// <param name="isAsync">is ascync or not</param>
        /// <returns>status of send mail</returns>
        public bool SendMail(string[] to, string subject, string templateName, Hashtable paraTable, string[] attachments, bool isAsync)
        {
            string template = GetTemplate(templateName, paraTable);
            return _mailHelper.SendMail(to, subject, template, attachments, isAsync);
        }

        //-------------- private methods

        /// <summary>
        /// get template by name
        /// </summary>
        /// <param name="templateName">name of template</param>
        /// <param name="paraTable">hashtable of paramaters</param>
        /// <returns></returns>
        private string GetTemplate(string templateName, Hashtable paraTable)
        {
            string template = XMLProvider.XMLDictionary.First(x => x.Key == ModuleName).Value[templateName];
            template = XMLProvider.ParseXML(template, paraTable);
            return template;
        }
    }
}
