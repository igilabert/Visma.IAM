
# Visma.IAM
Technical test for Visma Labs

## The problem:

Consider the following business rules:
- A user has: name, email, department and password
- A user can adopt one or more roles
- There is a hierarchy of roles.
- By default there are two roles: 
- Employee and Manager. Any role will also be an Employee.
- Users in a role have certain permissions
- The permissions can be: read, add, update and delete
- By default a Manager can perform all the operations
- By default an Employee can only read

The application must allow:
-	Register users. Verify that there is no other user with the same email.
-	Register roles. Be careful with the inheritance of roles. Don't define recursive roles.
-	Assign permissions to roles
-	Assign roles to users
-	Determine the permissions of a user based on her roles.
-	Notify the Manager that a user has been registered in their department


## The solution:

 **Since it is not clear for me the hierarchy of roles (and how to implement inheritance for the new roles) I decided to have a Employee role which will be the base class of the rest of employees. For the user permissions, I decided to take into account all the permissions that the user role have, not applying any hierarchy since I really don't know how to proceed, and I've been searching for a while. Sorry for the inconveniences.**

I've created an API to register users, roles, assign permissions to roles and roles to users.

The api consists of the following endpoints:
  
- ​GET /api​/Roles -> Lists the roles in the system. By default there are two roles in the system, Employee that can only read and Manager that can Create, Read, Update and Delete
- POST /api/Roles -> Adds new roles to the system. By default, if no permissions have been specified, the role will inherit the employee ones.
- PUT /api/Roles/{roleName} -> Applies the given permissions to the desired role, overwriting the previous ones.
- GET /api/Users -> Lis the roles in the system. By default there are no users.
- POST /api/Users -> Adds new users to the system. New users will be Employee by default. 
- GET /api/Users/{email} -> Gets the permissions of the desired user based on her roles.
- PUT /api/Users/{userMail}/{roleName} -> Adds a role to the user.

This solution does not allow to create a user with a role different from Employee. To add roles to the user, please use the available endpoint.

Data is not persisted into DB.

### Known issuses:
- No hierarchy at all.
- Api allows only to add permissions based on the enum (0 Create,1 Read,2 Update,3 Delete)
- When creating/Listing users, password appears. 
- Tests can be refactored
- Integration tests can be added (just tested it manually and with unit testing)
- Notification has not been implemented. We can just publish an event that a notification service will pick and notify the manager. Otherwise, it's simple to obtain the manager/s of the department where the user has been registered (via userService) and implement custom logic to send them an email.



# Answers

**Could you explain which design patterns have you used and what is the purpose of them?**
Singleton has been used in the dependency injection for both User and Role services to keep just one instance of those services. 
Dependency Injection, has been used also. 

**If we wanted to persist that information in any storage, could you please explain which type of storage you would choose and how it looks?**
After researching a bit, seems like the most used is a relational database, so I will choose AzureSQL (the one I'm most used to use).

The architecture will depend of the permissions.

 If they are fixed, just three tables:
- user: The primary key will be userId, not email. Email must be unique in the table also.
- roles: Will have roleId as primaryKey and holds the permissions for the role.
- pivot table userrole with userId and roleId as foreign key. Primary key would be role_id + userId. 

If the permissions are not fixed, two new tables will be need:
- permissions: with an entry for each permission
- pivot table with role_id and permission_id as foreign keys which will map the role with its permissions 


It should be worthy to think about implement another type of storage as an Azure Active Directory, LDAP etc.