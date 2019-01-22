using DynamicExpresso;
using Trepub.Common.Entities.Billings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Trepub.Common.Utils
{
    public class BillingCommon
    {
        public const string MyMobileOperator = "ApTel";
        public const string BillingIdentifier = "mb1";
        static public string NormalizeDialedNumber(string DialedNumber)
        {
            string MyRet = DialedNumber;
            if (MyRet.StartsWith("+"))
            {
                MyRet = "00" + MyRet.Remove(0, 1);
            }
            if (!MyRet.StartsWith("00") && MyRet.StartsWith("0"))//TODO: Handle 020 and ...
            {
                MyRet = "0098" + MyRet.Remove(0, 1);
            }
            return MyRet;
        }
        static public string getCallNetProvider(string NormalizedNumber)
        {
            if (!(NormalizedNumber.StartsWith("989") || NormalizedNumber.StartsWith("00989")))
            {
                return EntityConstants.CONST_NA;
            }
            var MobileOperators = new List<(string MNP, Regex PrefixRule)>()
            {
                ("ApTel", new Regex("^(00)?989991[13]")),
                ("AzarTel", new Regex("^(00)?9899914")),
                ("IranCell", new Regex("^(00)?989(3[0356789]|0[123])")),
                ("MCI", new Regex("^(00)?989(1|90|919)")),
                ("RightTel", new Regex("^(00)?9892[012]")),
                ("IranMobile", new Regex("^(00)?989")),
            };
            //new (string MNP, string PrefixRule)() { MNP = MobileNetworkProvider.ApTel, PrefixRule = new Regex("^(00)?989991[13]") },
            //new BaseClasses.MobileOperators() { MNP = MobileNetworkProvider.AzarTel, PrefixRule = new Regex("^(00)?9899914") },
            //new BaseClasses.MobileOperators() { MNP = MobileNetworkProvider.IranCell, PrefixRule = new Regex("^(00)?989(3[0356789]|0[123])") },
            //new BaseClasses.MobileOperators() { MNP = MobileNetworkProvider.MCI, PrefixRule = new Regex("^(00)?989(1|90|919)") },
            //new BaseClasses.MobileOperators() { MNP = MobileNetworkProvider.RightTel, PrefixRule = new Regex("^(00)?9892[012]") },
            //new BaseClasses.MobileOperators() { MNP = MobileNetworkProvider.IranMobile, PrefixRule = new Regex("^(00)?989") }
            foreach (var mo1 in MobileOperators)
            {
                if (mo1.PrefixRule.IsMatch(NormalizedNumber))
                {
                    return mo1.MNP;
                }
            }
            return EntityConstants.MOBILEOPERATORS_IRANMOBILE;
        }
        static public string getCallNetType(string NormalizedDialedNumber)
        {
            return EntityConstants.CONST_NA;
        }
        static public bool IsInDifferentMonths(DateTime st1, DateTime st2)
        {
            PersianCalendar pc = new PersianCalendar();
            return pc.GetMonth(st1) != pc.GetMonth(st2);
        }
        static public int GetDaysInMonth(DateTime st1)
        {
            PersianCalendar pc = new PersianCalendar();
            return pc.GetDaysInMonth(pc.GetYear(st1), pc.GetMonth(st1));
        }
        static public DateTime GetStartOfNextMonth(DateTime st1)
        {
            PersianCalendar pc = new PersianCalendar();
            var nextm1 = pc.AddMonths(st1, 1);
            return new DateTime(pc.GetYear(nextm1), pc.GetMonth(nextm1), 1, pc);//.AddSeconds(-1);
        }
        static public string GetCallSectorType(string NormalizedDialedNumber)
        {
            var ndn = NormalizedDialedNumber;
            if (ndn.StartsWith("00") && (!ndn.StartsWith($"0098")))
            {
                return EntityConstants.CALLSECTORTYPE_INTERNATIONAL;
            }
            //if (ndn.StartsWith("020") || (ndn.StartsWith("060")))//Handle BudgetInternational
            //{
            //    return CallNetType.BudgetInternational;
            //}
            if (ndn.StartsWith("00989"))
            {
                return EntityConstants.CALLSECTORTYPE_IRANMOBILE;
                //if (ndn.StartsWith("0098999"))//TODO: Move prefixes to config
                //{
                //    return CallNetType.MobileOnNet;
                //}
                //else
                //{
                //    return CallNetType.MobileOffNet;
                //}
            }
            return EntityConstants.CALLSECTORTYPE_SHORTCODE;
        }
        static public bool IsFarsiSMS(string CallType)
        {
            return (CallType.Equals("032") || CallType.Equals("033"));
        }
        static public bool IsLatinSMS(string CallType)
        {
            return (CallType.Equals("030") || CallType.Equals("031"));
        }
        static public bool IsCallForwading(string CallType)//TODO: Handle Call Forwarding
        {
            return (CallType.Equals("029"));
        }
        static public bool IsNationalInternet(string RecordType)
        {
            return false; //TODO: Correct this Item after clarification by MNO.
        }

    }
    public abstract class MyBilling
    {
        public CDR _CDR;
        public bool CurrentBoltonCriteriasPassed;
        public bool CurrentBoltonIsConsumed;
        public DateTime BillingEndTime;
        public MSISDN MS;
        public Bundle MSB;
        public BundleItem MSBI;
        public DefaultBoltonItem DBI;
        public HashSet<Bundle> NextMSBs;
        public HashSet<CDR> CDRs;
        public HashSet<CDR> CDRQ;
        public long AmountLeft;
        public Interpreter interpreter;
        public MyBilling()
        {
            CDRs = new HashSet<CDR>();
            CDRQ = new HashSet<CDR>();
            NextMSBs = new HashSet<Bundle>();
            CurrentBoltonCriteriasPassed = false;
            CurrentBoltonIsConsumed = false;
        }
        public bool CheckIfPartial()
        {
            var cdr1 = _CDR;
            //cdr1.ra = true;
            cdr1.IsPartial = cdr1.MNOAmountLeft > 0;
            return cdr1.IsPartial;
        }

        public DateTime GetMinDateTime(DateTime dt1, DateTime dt2)
        {
            return dt1 > dt2 ? dt2 : dt1;
        }
        public DateTime GetBillingEndTime(DateTime st1)
        {
            return BillingCommon.GetStartOfNextMonth(st1);
        }
        public void CalcWithBoltonItem()
        {
            var cdr1 = _CDR;
            var msbi1 = MSBI;
            var msb1 = MSB;
            if (cdr1.MNOAmount == 0)
            {
                cdr1.IsRated = true;
            }
            //TODO: Calc UsableDur and then Change UsableAmount based on usabledur/dur percentage (UsableAmaount=UsableAmaount*UsableDur/dur)
            //TODO: Use a variable as Upperbound limit time(For Reserve/Future Bolton for MSISDN)
            BillingEndTime = GetBillingEndTime(cdr1.StartTime);
            var BoltonItemRemainedAmount = msbi1.InitialAmount - msbi1.UsedAmount;
            var UsableAmount = (BoltonItemRemainedAmount >= cdr1.MNOAmount) ? cdr1.MNOAmount : (BoltonItemRemainedAmount);
            var EndTimeLimit = GetMinDateTime(GetMinDateTime(cdr1.EndTime, BillingEndTime), msb1.EndTime);
            //TODO: Make sure this critria(x.Bolton.Priority >= msb1.Bolton.Priority) is correct for all possible bolton mixtures, it is possible that the next bolton won't cover this cdr.
            var NextMSB = NextMSBs.Where(x => x.Bolton.Priority >= msb1.Bolton.Priority).OrderByDescending(x => x.Bolton.Priority).OrderBy(x => x.StartTime).FirstOrDefault();
            if (NextMSB != null)
            {
                EndTimeLimit = GetMinDateTime(EndTimeLimit, NextMSB.StartTime);
            }
            var UsableDuration = EndTimeLimit.Subtract(cdr1.StartTime).TotalSeconds;
            var cdrdur = cdr1.EndTime.Subtract(cdr1.StartTime).TotalSeconds;
            double MaxUsableAmountBasedOnDurLimit = UsableAmount;
            if (cdrdur > 0)
            {
                MaxUsableAmountBasedOnDurLimit = (UsableDuration / cdrdur) * cdr1.MNOAmount;
            }
            if (UsableAmount > MaxUsableAmountBasedOnDurLimit) { UsableAmount = (int)MaxUsableAmountBasedOnDurLimit; };
            if (UsableAmount > 0)
            {
                CurrentBoltonIsConsumed = true;
                //TODO: Covering Duration for Data against Bolton EndTime Must be Written
                cdr1.MNOAmountLeft = cdr1.MNOAmount - UsableAmount;
                cdr1.MSISDNAmount = UsableAmount;
                cdr1.MSISDNRate = 0;
                cdr1.MSISDNCost = (double)cdr1.MSISDNAmount * cdr1.MSISDNRate;
                msbi1.UsedAmount += UsableAmount;
                cdr1.BundleItem = msbi1;
                cdr1.BundleAmountLeft = msbi1.InitialAmount - msbi1.UsedAmount;
                cdr1.RatingType = EntityConstants.RATINGTYPE_BOLTONITEM;
                if (cdr1.MNOAmountLeft > 0)
                {
                    cdr1.EndTime = cdr1.StartTime.AddSeconds(UsableDuration);
                }
                if (msbi1.UsedAmount >= msbi1.InitialAmount)
                {
                    msb1.IsUsedUp = true;
                    msb1.UseUpTime = cdr1.EndTime;
                    msb1.MaxAmountsConsumptionPercentage = 100;
                    msb1.SetBoltonUsageCost();
                }
                cdr1.IsRated = true;
            }
        }
        public void CalcWithRate(double r1)
        {
            var cdr1 = _CDR;
            var dbi1 = DBI;
            cdr1.MNOAmountLeft = 0;
            cdr1.MSISDNAmount = cdr1.MNOAmount;
            cdr1.MSISDNRate = r1;
            cdr1.MSISDNCost = (double)cdr1.MSISDNAmount * cdr1.MSISDNRate;// / Units.VoiceTarrifUnit;
            cdr1.DefaultBoltonItem = dbi1;
            cdr1.RatingType = EntityConstants.RATINGTYPE_DEFBOLTONITEM;
            cdr1.IsRated = true;
        }
        public void CalcWithMNORate()
        {
            var cdr1 = _CDR;
            //cdr1.IsTriedToRate = true;
            cdr1.MNOAmountLeft = 0;
            cdr1.MSISDNAmount = cdr1.MNOAmount;
            cdr1.MSISDNRate = cdr1.MNORateOrg;
            cdr1.MSISDNCost = (double)cdr1.MSISDNAmount * cdr1.MSISDNRate;
            cdr1.RatingType = EntityConstants.RATINGTYPE_MNO;
            cdr1.IsRated = true;
        }
        public abstract void ContinueRatingWithAnotherCDR();
    }


    public class MyBillingData : MyBilling
    {
        public DataCDR CreateDataCDRFromRawDataCDR(RawDataCDR rawDataCDR)
        {
            var ret = new DataCDR();
            ret.RawDataCDR = rawDataCDR;
            ret.MobileNumber = rawDataCDR.Mobile_Number;
            //ret.DownloadAmount = long.Parse(RawDataCDR.Data_Volume_Incoming_1)
            //    + long.Parse(RawDataCDR.Data_Volume_Incoming_2)
            //    + long.Parse(RawDataCDR.Data_Volume_Incoming_3);//TODO: Correct ret
            //ret.UploadAmount = long.Parse(RawDataCDR.Data_Volume_Outgoing_1)
            //    + long.Parse(RawDataCDR.Data_Volume_Outgoing_2)
            //    + long.Parse(RawDataCDR.Data_Volume_Outgoing_3);//TODO: Correct ret
            ret.DownloadAmount = long.Parse(rawDataCDR.Data_Volume_Incoming_2);
            ret.UploadAmount = 0;
            ret.MNOAmountOrg = ret.DownloadAmount + ret.UploadAmount;
            ret.MNOAmount = ret.MNOAmountOrg;
            ret.MNOCostOrg = double.Parse(rawDataCDR.Dedicated_Amount_Used);//TODO: true Cost may be another field
            ret.MNOCost = ret.MNOCostOrg;
            ret.MNORateOrg = ret.MNOAmount == 0 ? 0 : ret.MNOCost / ret.MNOAmount;
            ret.StartTime = DateTime.ParseExact($"{rawDataCDR.Call_Date}", "yyyyMMddHHmmss", new CultureInfo("en-US"));
            ret.EndTime = ret.StartTime.AddSeconds(long.Parse(rawDataCDR.Call_Duration));
            ret.MNOAmountLeft = ret.MNOAmount;
            ret.APartyMNP = BillingCommon.getCallNetProvider(ret.MobileNumber);
            return ret;
        }
        public MyBillingData(RawDataCDR RawDataCDR)
        {
            this.CDR = CreateDataCDRFromRawDataCDR(RawDataCDR);
            base._CDR = this.CDR;
        }

        public override void ContinueRatingWithAnotherCDR()
        {
            //CDRs.Add(CDR);
            //var cdr1 = this.CDR;
            var cdr1 = CreateDataCDRFromRawDataCDR(CDR.RawDataCDR);
            cdr1.MNOAmountLeft = CDR.MNOAmountLeft;
            cdr1.MNOAmount = CDR.MNOAmountLeft;
            cdr1.MNOCost = CDR.MNOAmount * CDR.MNORateOrg;
            cdr1.MSISDN = CDR.MSISDN;//TODO: Make this(MSISDN assignment) better
            cdr1.StartTime = CDR.EndTime;//.AddSeconds(1);//TODO: correct this valus to end time of previous partial cdr record
            cdr1.IsPartial = true;
            CDRQ.Add(cdr1);
            //base._CDR = this.CDR;
        }
        public DataCDR CDR;
    }
    public class MyBillingVoice : MyBilling
    {
        public VoiceCDR CreateVoiceCDRFromRawVoiceCDR(RawVoiceCDR rawVoiceCDR)
        {
            var ret = new VoiceCDR();
            ret.RawVoiceCDR = rawVoiceCDR;
            ret.RawVoiceCDRId = rawVoiceCDR.RawVoiceCDRId;
            ret.MobileNumber = rawVoiceCDR.Charge_Party_Number;
            ret.DialedNumber = rawVoiceCDR.Dialed_Digit;
            ret.NormalizedDialedNumber = BillingCommon.NormalizeDialedNumber(rawVoiceCDR.Dialed_Digit);
            ret.MNOAmountOrg = long.Parse(rawVoiceCDR.Call_Duration);
            ret.MNOAmount = ret.MNOAmountOrg;
            ret.MNOCostOrg = double.Parse(rawVoiceCDR.Event_Amount);//TODO: RawVoiceCDR.Dedicated_Amount_Used
            ret.MNOCost = ret.MNOCostOrg;
            ret.MNORateOrg = ret.MNOAmount == 0 ? 0 : ((ret.MNOCost / ret.MNOAmount));//TODO: Correct Rate Formula
            ret.StartTime = DateTime.ParseExact($"{rawVoiceCDR.Call_Date}{rawVoiceCDR.Call_Time}", "yyyyMMddHHmmss", new CultureInfo("en-US"));
            ret.EndTime = ret.StartTime.AddSeconds(ret.MNOAmountOrg);
            ret.MNOAmountLeft = ret.MNOAmount;
            ret.RecordType = rawVoiceCDR.Record_Type;
            ret.CallType = rawVoiceCDR.Call_Type;
            ret.CallSectorType = BillingCommon.GetCallSectorType(ret.NormalizedDialedNumber);
            ret.APartyMNP = BillingCommon.getCallNetProvider(ret.MobileNumber);
            ret.BPartyMNP = BillingCommon.getCallNetProvider(ret.NormalizedDialedNumber);
            ret.CallNetType = ret.CallSectorType != EntityConstants.CALLSECTORTYPE_IRANMOBILE ? EntityConstants.CONST_NA : (ret.APartyMNP == ret.BPartyMNP ? EntityConstants.CALLNETTYPE_ONNET : EntityConstants.CALLNETTYPE_OFFNET);
            return ret;
        }
        public MyBillingVoice(RawVoiceCDR rawVoiceCDR)
        {
            this.CDR = CreateVoiceCDRFromRawVoiceCDR(rawVoiceCDR);
            base._CDR = this.CDR;
        }

        public bool IsIranMobileCall()
        {
            return CDR.CallSectorType == EntityConstants.CALLSECTORTYPE_IRANMOBILE;
        }
        public bool IsInterNationalCall()
        {
            return CDR.CallSectorType == EntityConstants.CALLSECTORTYPE_INTERNATIONAL;
        }
        public bool IsShortCodeCall()
        {
            return CDR.CallSectorType == EntityConstants.CALLSECTORTYPE_SHORTCODE;
        }
        public bool IsOnNetCall()
        {
            return CDR.CallNetType == EntityConstants.CALLNETTYPE_ONNET;
        }
        public bool IsOffNetCall()
        {
            return CDR.CallNetType == EntityConstants.CALLNETTYPE_OFFNET;
        }

        public override void ContinueRatingWithAnotherCDR()
        {
            //CDRs.Add(CDR);
            //var cdr1 = this.CDR;
            //CDR = new VoiceCDR(CDR.RawVoiceCDR);
            //CDR.MNOAmountLeft = cdr1.MNOAmountLeft;
            //CDR.MNOAmount = cdr1.MNOAmountLeft;
            ////VC.MNORate = cdr1.MNORate;
            //CDR.MNOCost = CDR.MNOAmount * CDR.MNORateOrg;
            //CDR.MSISDN = cdr1.MSISDN;//TODO: Make this(MSISDN assignment) better
            //CDR.StartTime = cdr1.EndTime;//.AddSeconds(1);//TODO: correct this valus to end time of previous partial cdr record
            //base._CDR = this.CDR;
            var cdr1 = CreateVoiceCDRFromRawVoiceCDR(CDR.RawVoiceCDR);
            cdr1.MNOAmountLeft = CDR.MNOAmountLeft;
            cdr1.MNOAmount = CDR.MNOAmountLeft;
            cdr1.MNOCost = CDR.MNOAmount * CDR.MNORateOrg;
            cdr1.MSISDN = CDR.MSISDN;//TODO: Make this(MSISDN assignment) better
            cdr1.StartTime = CDR.EndTime;//.AddSeconds(1);//TODO: correct this valus to end time of previous partial cdr record
            cdr1.IsPartial = true;
            CDRQ.Add(cdr1);
        }
        public VoiceCDR CDR;
    }
    public class MyBillingSMS : MyBilling
    {
        public SMSCDR CreateSMSCDRFromRawSMSCDR(RawSMSCDR rawSMSCDR)
        {
            var ret = new SMSCDR();
            ret.RawSMSCDR = rawSMSCDR;
            ret.MobileNumber = rawSMSCDR.Charge_Party_Number;
            ret.DialedNumber = rawSMSCDR.Dialed_Digit;
            ret.NormalizedDialedNumber = BillingCommon.NormalizeDialedNumber(rawSMSCDR.Dialed_Digit);
            ret.MNOAmountOrg = 1; // long.Parse(RawSMSCDR.Call_Duration);//TODO: Correct Amount
            ret.MNOAmount = ret.MNOAmountOrg;
            ret.MNOCostOrg = double.Parse(rawSMSCDR.Event_Amount);// double.Parse(RawSMSCDR.Dedicated_Amount_Used);//TODO: Mapping Fld is Correct?
            ret.MNOCost = ret.MNOCostOrg;
            ret.MNORateOrg = ret.MNOAmount == 0 ? 0 : (ret.MNOCost / (ret.MNOAmount));//TODO: Correct Rate Formula
            ret.StartTime = DateTime.ParseExact($"{rawSMSCDR.Call_Date}{rawSMSCDR.Call_Time}", "yyyyMMddHHmmss", new CultureInfo("en-US"));
            ret.EndTime = ret.StartTime.AddSeconds(ret.MNOAmountOrg);
            ret.MNOAmountLeft = ret.MNOAmount;
            ret.RecordType = rawSMSCDR.Record_Type;
            ret.CallType = rawSMSCDR.Call_Type;
            ret.CallSectorType = BillingCommon.GetCallSectorType(ret.NormalizedDialedNumber);
            ret.APartyMNP = BillingCommon.getCallNetProvider(ret.MobileNumber);
            ret.BPartyMNP = BillingCommon.getCallNetProvider(ret.NormalizedDialedNumber);
            ret.CallNetType = ret.CallSectorType != EntityConstants.CALLSECTORTYPE_IRANMOBILE ? EntityConstants.CONST_NA : (ret.APartyMNP == ret.BPartyMNP ? EntityConstants.CALLNETTYPE_ONNET : EntityConstants.CALLNETTYPE_OFFNET);
            return ret;
        }
        public MyBillingSMS(RawSMSCDR RawSMSCDR)
        {
            this.CDR = CreateSMSCDRFromRawSMSCDR(RawSMSCDR);
            base._CDR = this.CDR;
        }

        public bool IsIranMobileCall()
        {
            return CDR.CallSectorType == EntityConstants.CALLSECTORTYPE_IRANMOBILE;
        }
        public bool IsInterNationalCall()
        {
            return CDR.CallSectorType == EntityConstants.CALLSECTORTYPE_INTERNATIONAL;
        }
        public bool IsShortCodeCall()
        {
            return CDR.CallSectorType == EntityConstants.CALLSECTORTYPE_SHORTCODE;
        }
        public bool IsOnNetCall()
        {
            return CDR.CallNetType == EntityConstants.CALLNETTYPE_ONNET;
        }
        public bool IsOffNetCall()
        {
            return CDR.CallNetType == EntityConstants.CALLNETTYPE_OFFNET;
        }
        public bool IsFarsiSMS()
        {
            return BillingCommon.IsFarsiSMS(CDR.CallType);
        }
        public bool IsLatinSMS()
        {
            return BillingCommon.IsLatinSMS(CDR.CallType);
        }

        public override void ContinueRatingWithAnotherCDR()
        {
            //CDRs.Add(CDR);
            //var cdr1 = this.CDR;
            //CDR = new SMSCDR(CDR.RawSMSCDR);
            //CDR.MNOAmountLeft = cdr1.MNOAmountLeft;
            //CDR.MNOAmount = cdr1.MNOAmountLeft;
            ////VC.MNORate = cdr1.MNORate;
            //CDR.MNOCost = CDR.MNOAmount * CDR.MNORateOrg;
            //CDR.MSISDN = cdr1.MSISDN;//TODO: Make this(MSISDN assignment) better
            //CDR.StartTime = cdr1.EndTime;//.AddSeconds(1);//TODO: correct this valus to end time of previous partial cdr record
            //base._CDR = this.CDR;
            var cdr1 = CreateSMSCDRFromRawSMSCDR(CDR.RawSMSCDR);
            cdr1.MNOAmountLeft = CDR.MNOAmountLeft;
            cdr1.MNOAmount = CDR.MNOAmountLeft;
            cdr1.MNOCost = CDR.MNOAmount * CDR.MNORateOrg;
            cdr1.MSISDN = CDR.MSISDN;//TODO: Make this(MSISDN assignment) better
            cdr1.StartTime = CDR.EndTime;//.AddSeconds(1);//TODO: correct this valus to end time of previous partial cdr record
            cdr1.IsPartial = true;
            CDRQ.Add(cdr1);
        }
        public SMSCDR CDR;
    }
}
