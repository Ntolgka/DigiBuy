﻿dotnet ef migrations list --startup-project ".\DigiBuy.Api\"
dotnet ef database update 20240805200032_InitialCreate --startup-project ".\DigiBuy.Api\"

dotnet ef database update --startup-project ".\DigiBuy.Api\"

dotnet ef migrations add InitialCreate -s ..\DigiBuy.Api 

dotnet ef migrations add RemoveIsActiveUserProp -s ..\DigiBuy.Api 

User = 1, Admin = 0
Active = 0, Inactive = 1

Console.WriteLine($"\n \n \n \n \n  Retrieved from cache: {cacheKey} \n \n \n \n \n");

{
  "UserName": "tolga",
  "Password": "Tolga_2000"
}

{
  "CardNumber": "4766190711111111",
  "CardHolderName": "Tolga Nalbant",
  "ExpiryMonth": "11",
  "ExpiryYear": "2030",
  "CVV": "111"
}