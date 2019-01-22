USE trepub;

-- default roles 
INSERT INTO roleitem (RoleItemID, Name, Status) VALUES (1, 'کاربر معمولی', 'AC');
INSERT INTO roleitem (RoleItemID, Name, Status) VALUES (2, 'مدیر سایت', 'AC');

-- default parties
INSERT INTO party (PartyId, Name, LastName, Type, Status)
VALUES (1, 'دالیب', null, 'BU', 'AC');

-- default admin profile
INSERT INTO profile (ProfileID, UserID, Type, Password, Status, MobileNumber, PartyId)
VALUES (1, 'administrator', 'ID', 'ngYqNk7FWK2Ya8NfQbABptRWhU3lwP1+TNOEyABUaUc=', 'AC', '99999999999', 1);
INSERT INTO profilerole (ProfileID, RoleItemID, Status)
VALUES (1, 1, 'AC');
INSERT INTO profilerole (ProfileID, RoleItemID, Status)
VALUES (1, 2, 'AC');

-- permissions
INSERT INTO resource (ResourceID, ResourceContent, Name, Type, Status)
VALUES (1, '^/api/profiles/getMine(/)?$', 'خواندن جزییات پروفایل خود', 'AP', 'AC');
INSERT INTO roleitemresource (ResourceID, RoleItemID, Status)
VALUES (1, 1, 'AC');
INSERT INTO roleitemresource (ResourceID, RoleItemID, Status)
VALUES (1, 2, 'AC');

INSERT INTO resource (ResourceID, ResourceContent, Name, Type, Status)
VALUES (2, '^/api/profiles/updateMine(/)?$', 'بروز رسانی پروفایل خود', 'AP', 'AC');
INSERT INTO roleitemresource (ResourceID, RoleItemID, Status)
VALUES (2, 1, 'AC');
INSERT INTO roleitemresource (ResourceID, RoleItemID, Status)
VALUES (2, 2, 'AC');

INSERT INTO resource (ResourceID, ResourceContent, Name, Type, Status)
VALUES (3, 'api/profiles/deleteMine(/)?$', 'حذف پروفایل خود', 'AP', 'AC');
INSERT INTO roleitemresource (ResourceID, RoleItemID, Status)
VALUES (3, 1, 'AC');
INSERT INTO roleitemresource (ResourceID, RoleItemID, Status)
VALUES (3, 2, 'AC');

INSERT INTO resource (ResourceID, ResourceContent, Name, Type, Status)
VALUES (4, 'api/profiles/list(/)?$', 'دریافت فهرستت پروفایل ها', 'AP', 'AC');
INSERT INTO roleitemresource (ResourceID, RoleItemID, Status)
VALUES (4, 2, 'AC');

INSERT INTO resource (ResourceID, ResourceContent, Name, Type, Status)
VALUES (5, '^/api/profiles/changePasswordMine(/)?$', 'تغییر رمز عبور خود', 'AP', 'AC'); 	
INSERT INTO roleitemresource (ResourceID, RoleItemID, Status)
VALUES (5, 1, 'AC');
INSERT INTO roleitemresource (ResourceID, RoleItemID, Status)
VALUES (5, 2, 'AC');



