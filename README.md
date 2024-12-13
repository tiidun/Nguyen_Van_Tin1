# UrlShortener C# App + RESTful API

Summary: UrlShortener allows users to **manage their own short links!** Create, edit, and delete short links of your **favourite websites!**

- Target platform: .NET 6
- Seeded database with one user and three addresses
- Default user credentials: `guest@mail.com` / `guest123`

## UrlShortener Web App

The ASP.NET Core app "UrlShortener" is an app for **making short URLs**.

- Technologies: C#, ASP.NET Core, Entity Framework Core, ASP.NET Core Identity, NUnit
- The app supports the following operations:
  - Home page (view latest created / your own short URls): `/`
  - **View** addresses: `/URLAddress/All`
  - **Create** a new short URL (URL + short code): `/URLAddress/Add`
  - **Edit** addresse: `/URLAddress/Edit/:id`
  - **Delete** addresse: `/URLAddress/Delete/:id`

## UrlShortener RESTful API

The following endpoints are supported:

- `GET /api` - list all API endpoints
- `GET /api/urladdresses` - list all addresses
- `GET /api/urladdresses/count` - returns address count
- `GET /api/urladdresses/search/:keyword` - returns an URL that match the given keyword
- `POST /api/urladdresses/create` - create a new short URL (send a JSON object in the request body, e.g. `{ "URL": "https://www.google.com", "Short Code": "goog" }`)
- `PUT /api/urladdresses/:originalUrl` - edit address by its original URL (send a JSON object in the request body, holding all fields, e.g. `{ "URL": "https://www.google.com", "Short Code": "goog" }`)
- `DELETE /api/urladdresses/:id` - delete address by `id`
- `POST /api/users/login` - logs in an existing user (send a JSON object in the request body, holding all fields, e.g. `{ "email": "guest@mail.com", "password": "guest123" }`)
- `POST /api/users/register` - registers a new user (send a JSON object in the request body, holding all fields, e.g. `{ "email": "someUsername@mail.bg", "password": "somePassword" }`)

## Screenshots

![home-page](https://github.com/SoftUni/ShortURL/assets/72888249/ff511d23-9f03-41ae-8bb3-375021495075)
![register-page](https://github.com/SoftUni/ShortURL/assets/72888249/ae3acba2-10df-4a00-a491-c034814080a0)
![login-page](https://github.com/SoftUni/ShortURL/assets/72888249/43b8d7fb-2a10-452f-8c1b-bb7184b33ef5)
![home-page-logged-in](https://github.com/SoftUni/ShortURL/assets/72888249/bb2289f0-73fc-4cfd-bc36-ed97a339640e)
![all-addresses](https://github.com/SoftUni/ShortURL/assets/72888249/eef04f19-d9f8-4b90-afde-cb75dd058be4)
![create-address](https://github.com/SoftUni/ShortURL/assets/72888249/997df6e6-1ded-432b-bdd2-fd198790e72a)
![edit-address](https://github.com/SoftUni/ShortURL/assets/72888249/97cb50a8-a0f0-42b5-aad8-9d7b709800ec)
![delete-address](https://github.com/SoftUni/ShortURL/assets/72888249/d515f403-9a8f-4690-81e7-8ffd8f641e4d)
