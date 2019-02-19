USE trepubdotnet;

-- default roles 
INSERT INTO roleitem (RoleItemID, Name, Status) VALUES (1, 'normal user', 'AC');
INSERT INTO roleitem (RoleItemID, Name, Status) VALUES (2, 'admin', 'AC');

-- default parties
INSERT INTO party (PartyId, Name, LastName, Type, Status)
VALUES (1, 'trepub', null, 'BU', 'AC');

-- default admin profile
INSERT INTO profile (ProfileID, UserID, Type, Password, Status, MobileNumber, PartyId)
VALUES (1, 'administrator', 'ID', 'ONKJSvFP+S7ymY8Q1UMLwokt1wYhoBjXbcSbX4vSNHY=', 'AC', '99999999999', 1);
INSERT INTO profilerole (ProfileID, RoleItemID, Status)
VALUES (1, 1, 'AC');
INSERT INTO profilerole (ProfileID, RoleItemID, Status)
VALUES (1, 2, 'AC');

-- permissions
INSERT INTO resource (ResourceID, ResourceContent, Name, Type, Status)
VALUES (1, '^/api/profiles/getMine(/)?$', 'get my profile', 'AP', 'AC');
INSERT INTO roleitemresource (ResourceID, RoleItemID, Status)
VALUES (1, 1, 'AC');
INSERT INTO roleitemresource (ResourceID, RoleItemID, Status)
VALUES (1, 2, 'AC');

INSERT INTO resource (ResourceID, ResourceContent, Name, Type, Status)
VALUES (2, '^/api/profiles/updateMine(/)?$', 'update my profile', 'AP', 'AC');
INSERT INTO roleitemresource (ResourceID, RoleItemID, Status)
VALUES (2, 1, 'AC');
INSERT INTO roleitemresource (ResourceID, RoleItemID, Status)
VALUES (2, 2, 'AC');

INSERT INTO resource (ResourceID, ResourceContent, Name, Type, Status)
VALUES (3, 'api/profiles/deleteMine(/)?$', 'delete my profile', 'AP', 'AC');
INSERT INTO roleitemresource (ResourceID, RoleItemID, Status)
VALUES (3, 1, 'AC');
INSERT INTO roleitemresource (ResourceID, RoleItemID, Status)
VALUES (3, 2, 'AC');

INSERT INTO resource (ResourceID, ResourceContent, Name, Type, Status)
VALUES (4, 'api/profiles/list(/)?$', 'get profile list', 'AP', 'AC');
INSERT INTO roleitemresource (ResourceID, RoleItemID, Status)
VALUES (4, 2, 'AC');

INSERT INTO resource (ResourceID, ResourceContent, Name, Type, Status)
VALUES (5, '^/api/profiles/changePasswordMine(/)?$', 'change my password', 'AP', 'AC'); 	
INSERT INTO roleitemresource (ResourceID, RoleItemID, Status)
VALUES (5, 1, 'AC');
INSERT INTO roleitemresource (ResourceID, RoleItemID, Status)
VALUES (5, 2, 'AC');



