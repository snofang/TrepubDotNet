using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Trepub.Common.Utils
{
    public class MobileNumberUtils
    {
        protected static string MSISDN_PATTERNS_ZEROLEADING = @"^09\d{9}$";
        protected static string MSISDN_PATTERNS_COUNTRYCODELEADING = @"^989(\d){9}$";
        protected static string MSISDN_PATTERNS_PLUSCOUNTRYCODELEADING = @"^\+989(\d){9}$";

        public static string NormalizeTo98Leading(string mobileNumber)
        {
            //Normalize to 989991234567 like format
            if (!string.IsNullOrEmpty(mobileNumber) && !mobileNumber.StartsWith("00") && mobileNumber.StartsWith("0"))//TODO: Maa, Handle 020 and ...
            {
                mobileNumber = "98" + mobileNumber.Remove(0, 1);
            }
            else if (!string.IsNullOrEmpty(mobileNumber) && mobileNumber.StartsWith("0098"))
            {
                mobileNumber = mobileNumber.Remove(0, 2);
            }
            else if (!string.IsNullOrEmpty(mobileNumber) && mobileNumber.StartsWith("+98"))
            {
                mobileNumber = mobileNumber.Remove(0, 1);
            }
            return mobileNumber;
        }

        public static string NormalizeToZeroLeading(string mobileNumber)
        {
            var format = GetMsisdnFormat(mobileNumber);
            switch (format)
            {
                case MsisdnFormats.ZeroLeading:
                    return mobileNumber;
                case MsisdnFormats.CountryCodeLeading:
                    return "0" + mobileNumber.Remove(0, 2);
                default:
                    throw new Exception($"Invalid MSISDN format; {mobileNumber}");
            }
        }

        public static string NormalizeToPlusCountryCodeLeading(string mobileNumber)
        {
            var format = GetMsisdnFormat(mobileNumber);
            switch (format)
            {
                case MsisdnFormats.ZeroLeading:
                    return "+98" + mobileNumber.Substring(1);
                case MsisdnFormats.CountryCodeLeading:
                    return "+" + mobileNumber;
                case MsisdnFormats.PlusCountryCodeLeading:
                    return mobileNumber;
                default:
                    throw new Exception($"Invalid MSISDN format; {mobileNumber}");
            }
        }


        public static MsisdnFormats GetMsisdnFormat(string number)
        {
            if (Regex.IsMatch(number, MSISDN_PATTERNS_ZEROLEADING, RegexOptions.IgnoreCase))
            {
                return MsisdnFormats.ZeroLeading;
            }
            else if (Regex.IsMatch(number, MSISDN_PATTERNS_COUNTRYCODELEADING, RegexOptions.IgnoreCase))
            {
                return MsisdnFormats.CountryCodeLeading;
            }
            else if (Regex.IsMatch(number, MSISDN_PATTERNS_PLUSCOUNTRYCODELEADING, RegexOptions.IgnoreCase))
            {
                return MsisdnFormats.PlusCountryCodeLeading;
            }
            return MsisdnFormats.Unknown;
        }

        public enum MsisdnFormats
        {
            ZeroLeading,
            CountryCodeLeading,
            PlusCountryCodeLeading,
            Unknown,
        }
    }
}
