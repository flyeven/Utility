﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;

#pragma warning  disable 1591
/* From https://github.com/RoyiNamir/SimplePostVariableParameterBindingExtended
 * 
 * 修改  
 * 1.修饰了特性MultiParameterSupportAttribute的Action，才使用此绑定器处理。
 *   搜索关键词：“Here添加Code”。
 * 2.修改Json反序列化时，调整为Object
 *   搜索关键词：“Here调整Code”
 * 
 * 后续升级的时候，从github下载最新代码，并补充上述两行修改即可。
 */


namespace Utility.ThirdpartUtility.WebApiExtension
{
    /// <summary>
    /// 支持多参数的参数处理绑定器<para />
    /// (具体用法，请参数 ReadMe.txt )
    /// </summary>
    public class SimplePostVariableParameterBinding : HttpParameterBinding
    {
        public const string MultipleBodyParameters = "MultipleBodyParameters";
        public const string MultipleBodyParametersRaw = "MultipleBodyParametersRaw";
        public const bool AllowJsonContentType = true;


        public SimplePostVariableParameterBinding(HttpParameterDescriptor descriptor)
            : base(descriptor)
        {
        }

        /// <summary>
        /// Check for simple binding parameters in POST data. Bind POST
        /// data as well as query string data
        /// </summary>
        /// <param name="metadataProvider"></param>
        /// <param name="actionContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task ExecuteBindingAsync(ModelMetadataProvider metadataProvider,
                                                    HttpActionContext actionContext,
                                                    CancellationToken cancellationToken)
        {
            string stringValue = null;

            NameValueCollection col = TryReadBody(actionContext.Request);
            if (col != null)
                stringValue = col[Descriptor.ParameterName];

            // try reading query string if we have no POST/PUT match
            if (stringValue == null)
            {
                var query = actionContext.Request.GetQueryNameValuePairs();
                if (query != null)
                {
                    var matches = query.Where(kv => kv.Key.ToLower() == Descriptor.ParameterName.ToLower());
                    var keyValuePairs = matches as IList<KeyValuePair<string, string>> ?? matches.ToList();
                    if (keyValuePairs.Any())
                        stringValue = keyValuePairs.First().Value;
                }
            }

            object value = StringToType(stringValue);

            // Set the binding result here
            SetValue(actionContext, value);

            // now, we can return a completed task with no result
            TaskCompletionSource<AsyncVoid> tcs = new TaskCompletionSource<AsyncVoid>();
            tcs.SetResult(default(AsyncVoid));
            return tcs.Task;
        }


        /// <summary>
        /// Method that implements parameter binding hookup to the global configuration object's
        /// ParameterBindingRules collection delegate.
        /// 
        /// This routine filters based on POST/PUT method status and simple parameter
        /// types.
        /// </summary>
        /// <example>
        /// GlobalConfiguration.Configuration.
        ///       .ParameterBindingRules
        ///       .Insert(0,SimplePostVariableParameterBinding.HookupParameterBinding);
        /// </example>    
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public static HttpParameterBinding HookupParameterBinding(HttpParameterDescriptor descriptor)
        {
            var supportedMethods = descriptor.ActionDescriptor.SupportedHttpMethods;

            //Here添加Code--------

            if (descriptor.ActionDescriptor.GetCustomAttributes<MultiParameterSupportAttribute>().Count <= 0)
                return null;

            //--------------------

            // Only apply this binder on POST and PUT operations
            if (supportedMethods.Contains(HttpMethod.Post) ||
                supportedMethods.Contains(HttpMethod.Put))
            {
                var supportedTypes = new Type[] { typeof(string), 
                                                typeof(int), 
                                                typeof(int?), 
                                                typeof(decimal), 
                                                typeof(decimal?), 
                                                typeof(double), 
                                                typeof(double?), 
                                                typeof(long), 
                                                typeof(long?), 
                                                typeof(bool),
                                                typeof(bool?),
                                                typeof(DateTime),
                                                typeof(DateTime?),
                                                typeof(byte[])
                                            };

                if (supportedTypes.Count(typ => typ == descriptor.ParameterType) > 0)
                    return new SimplePostVariableParameterBinding(descriptor);
            }

            return null;
        }


        object StringToType(string stringValue)
        {
            object value = null;

            if (stringValue == null) value = null;
            else if (Descriptor.ParameterType == typeof(string)) value = stringValue;
            else if (Descriptor.ParameterType == typeof(int)) value = int.Parse(stringValue, CultureInfo.CurrentCulture);

            else if (Descriptor.ParameterType == typeof(int?)) value = string.IsNullOrWhiteSpace(stringValue) ? (int?)null : int.Parse(stringValue, CultureInfo.CurrentCulture);
            else if (Descriptor.ParameterType == typeof(long)) value = long.Parse(stringValue, CultureInfo.CurrentCulture);
            else if (Descriptor.ParameterType == typeof(long?)) value = string.IsNullOrWhiteSpace(stringValue) ? (long?)null : long.Parse(stringValue, CultureInfo.CurrentCulture);
            else if (Descriptor.ParameterType == typeof(decimal)) value = decimal.Parse(stringValue, CultureInfo.CurrentCulture);
            else if (Descriptor.ParameterType == typeof(decimal?)) value = string.IsNullOrWhiteSpace(stringValue) ? (decimal?)null : decimal.Parse(stringValue, CultureInfo.CurrentCulture);
            else if (Descriptor.ParameterType == typeof(double)) value = double.Parse(stringValue, CultureInfo.CurrentCulture);
            else if (Descriptor.ParameterType == typeof(double?)) value = string.IsNullOrWhiteSpace(stringValue) ? (double?)null : double.Parse(stringValue, CultureInfo.CurrentCulture);
            else if (Descriptor.ParameterType == typeof(DateTime)) value = DateTime.Parse(stringValue, CultureInfo.CurrentCulture);
            else if (Descriptor.ParameterType == typeof(DateTime?)) value = string.IsNullOrWhiteSpace(stringValue) ? (DateTime?)null : DateTime.Parse(stringValue, CultureInfo.CurrentCulture);
            else if (Descriptor.ParameterType == typeof(bool))
            {
                value = false;
                if (stringValue == "true" || stringValue == "on" || stringValue == "1") value = true;
            }
            else if (Descriptor.ParameterType == typeof(bool?))
            {
                value = false;
                if (string.IsNullOrWhiteSpace(stringValue)) value = (bool?)null;
                else
                    if (stringValue == "true" || stringValue == "on" || stringValue == "1") value = true;
            }
            else value = stringValue;

            return value;
        }

        /// <summary>
        /// Read and cache the request body
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private NameValueCollection TryReadBody(HttpRequestMessage request)
        {
            object result;

            // try to read out of cache first
            if (!request.Properties.TryGetValue(MultipleBodyParameters, out result))
            {
                var contentType = request.Content.Headers.ContentType;



                if (contentType != null)

                    switch (contentType.MediaType)
                    {
                        case "application/json":
                            {
                                if (AllowJsonContentType)
                                {
                                    result = request.Content.ReadAsStringAsync().Result;
                                    request.Properties.Add(MultipleBodyParametersRaw, result);

                                    //Here调整Code-------

                                    var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(result.ToString());
                                    result = values.Aggregate(new NameValueCollection(),
                                                              (seed, current) =>
                                                              {
                                                                  seed.Add(current.Key, current.Value == null ? "" : current.Value.ToString());
                                                                  return seed;
                                                              });

                                    //var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.ToString());
                                    //result = values.Aggregate(new NameValueCollection(),
                                    //                          (seed, current) =>
                                    //                          {
                                    //                              seed.Add(current.Key, current.Value);
                                    //                              return seed;
                                    //                          });

                                    //------------------


                                    request.Properties.Add(MultipleBodyParameters, result);
                                }

                            }
                            break;
                        case "application/x-www-form-urlencoded":
                            result = request.Content.ReadAsFormDataAsync().Result;
                            request.Properties.Add(MultipleBodyParameters, result);
                            break;
                    }
            }


            return result as NameValueCollection;
        }

        private struct AsyncVoid
        {
        }
    }
}
