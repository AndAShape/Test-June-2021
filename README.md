# Setup

Project _DataAccess.IntegrationTests_ requires a new top-level file _config.Development.json_. Its contents should duplicate _config.json_ but with the values from the requirements doc added for the properties. (_BaseUrl_ refers to the customer API and is everything before _GetUserDetails_ - it should include the trailing forward slash.)

Similarly, project _Web_ requires a new file _appsettings.Development.json_.

This project's API method is at _/orders/recent_.

# Improvements

- Better formatting of delivery address, which takes into account missing fields.
- Only require email address parameter. The customer Id is redundant because it's looked up from the email. If the customer Id is a relatively secret value that is supposed to prove the legitimacy of the request the then it would be better to authenticate using a token.
- Proper dependency injection container.
- Caching
- Logging
- Production instance would need a production config file.

