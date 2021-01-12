using SmsIrRestful;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Helpers
{
    public class SendSms
    {
        public static void SendCommonSms(string cellNumber, string message)
        {
            var token = new Token().GetToken("cb6f10e0147b27dbea1b14df", "123qwe!@#QWE");

            SmsIrRestful.MessageSend messageSend = new SmsIrRestful.MessageSend();

            var res = messageSend.Send(token, new SmsIrRestful.MessageSendObject()
            {
                MobileNumbers = new List<string>() { cellNumber }.ToArray(),
                Messages = new List<string>() { message }.ToArray(),
                LineNumber = "30005240003000",
                SendDateTime = null,
                CanContinueInCaseOfError = false
            });

            SmsIrRestful.Credit credit = new SmsIrRestful.Credit();
            SmsIrRestful.CreditResponse creditResponse = new SmsIrRestful.CreditResponse();

        }
        public static void SendOtpSms(string cellNumber, string code)
        {
            var token = new Token().GetToken("cb6f10e0147b27dbea1b14df", "123qwe!@#QWE");

            var ultraFastSend = new UltraFastSend()
            {
                Mobile = Convert.ToInt64(cellNumber),
                TemplateId = 34939,
                ParameterArray = new List<UltraFastParameters>()
                {
                    new UltraFastParameters()
                    {
                        Parameter = "verifyCode" , ParameterValue = code
                    }
                }.ToArray()

            };

            UltraFastSendRespone ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);

            if (ultraFastSendRespone.IsSuccessful)
            {

            }
            else
            {

            }
        }

    }

    
}