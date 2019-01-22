using System;
using System.Collections.Generic;
using System.Text;

namespace Trepub.Common
{
    public class EntityConstants
    {

        public const int PROFILE_ID_ADMIN = 1;

        public const int PARTY_ID_Trepub = 1;
        public const string PARTY_TYPE_PERSON = "PE";
        public const string PARTY_TYPE_BUSINESSUNIT = "BU";
        public const string PARTY_STATUS_ACTIVE = "AC";


        public const string IDMAP_PROFILEID = "IDMAP_PROFILEID";
        public const string IDMAP_FILERESOURCEID = "IDMAP_FILERESOURCEID";
        public const string IDMAP_ACCOUNTID = "IDMAP_ACCOUNTID";
        public const string IDMAP_PLAYID = "IDMAP_PLAYID";
        public const string IDMAP_PARTYID = "IDMAP_PARTYID";
        public const string IDMAP_TRANSACTIONID = "IDMAP_TRANSACTIONID";


        public const string PROFILE_TYPE_ID = "ID";
        public const string PROFILE_TYPE_MOBILENUMBER = "MO";
        public const string PROFILE_TYPE_FREE = "FR";

        public const string PROFILE_STATUS_ACTIVE = "AC";
        public const string PROFILE_STATUS_DEACTIVE = "DA";
        public const string PROFILE_STATUS_DISABLE = "DI";

        public const string RESOURCE_STATUS_ACTIVE = "AC";
        public const string RESOURCE_STATUS_DEACTIVE = "DA";

        public const string PROFILEROLE_STATUS_ACTIVE = "AC";


        public const int ROLE_IDS_NORMALUSER = 1;
        public const int ROLE_IDS_ADMINISTRATOR = 2;
        public const string ROLEITEM_STATUS_ACTIVE = "AC";
        public const string ROLEPROFILE_STATUS_ACTIVE = "AC";


        //PARTY CONFIG'S VALUE
        public const string SMS_SENDER_AP = "AP";
        public const string SMS_SENDER_NT = "NT";

        public const string FILERESOURCE_STATUS_INITIAL = "IN";
        public const string FILERESOURCE_STATUS_PROCESSING = "PR";
        public const string FILERESOURCE_STATUS_COMPLETED = "CP";

        public const string FILERESOURCE_TYPE_VIDEO = "VI";

        //account
        public const int ACCOUNT_ID_Trepub = 1;
        public const string ACCOUNT_TYPE_FILERESOURCEACCOUNT = "FA";
        public const string ACCOUNT_TYPE_PARTYACCOUNT = "PA";
        public const string ACCOUNT_STATUS_ACTIVE = "AC";

        //play
        public const string PLAY_STATUS_ACTIVE = "AC";
        public const string PLAY_STATUS_DEACTIVE = "DA";

        //transaction
        public const string TRANSACTION_STATUS_CREATED = "CR";
        public const string TRANSACTION_STATUS_PROCESSED = "PR";
        public const string TRANSACTION_TYPE_PLAY = "PL";
        public const string TRANSACTION_CREDITEBIT_CREDIT = "CR";
        public const string TRANSACTION_CREDITEBIT_DEBIT = "DB";

        //price
        public const int PRICE_ID_DEFAULT = 1;
        public const string PRICE_TYPECODE_TAXPERCENT = "TP";

    }
}
