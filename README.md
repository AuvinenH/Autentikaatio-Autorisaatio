# Autentikaatio-Autorisaatio

JWT-autentikoinnin ja claims/roolipohjaisen autorisoinnin demo ASP.NET Core Web API -projektilla (.NET 8).

## Mitä projektissa on

- JWT-autentikointi (`Bearer`) konfiguroituna `Program.cs`-tiedostossa.
- Swagger-konfiguraatio, jossa token voidaan syöttää `Authorize`-painikkeen kautta.
- Demo-login endpoint tokenin generointiin.
- Kaksi käyttäjäroolia tokenissa claimsien avulla:
	- `User`
	- `Admin`
- Policy-pohjainen suojaus endpointille `RequireAdminRole`.

## Teknologiat

- .NET 8 Web API
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `System.IdentityModel.Tokens.Jwt`
- Swagger / OpenAPI (`Swashbuckle.AspNetCore`)

## Konfiguraation ydin

- `Program.cs`
	- `AddAuthentication().AddJwtBearer(...)`
	- `AddAuthorization(...)` policyllä `RequireAdminRole`
	- middleware-järjestys:
		- `app.UseAuthentication();`
		- `app.UseAuthorization();`
- `services/TokenService.cs`
	- Luo JWT-tokenin ja lisää claimit:
		- `ClaimTypes.Name`
		- `ClaimTypes.Role` (`Admin` tai `User`)

## Endpointit

Base route: `/WeatherForecast`

- `GET /WeatherForecast/OpenGet`
	- Avoin endpoint (`[AllowAnonymous]`)
- `GET /WeatherForecast/AuthGet`
	- Vaatii validin JWT-tokenin (`[Authorize]`)
- `POST /WeatherForecast/login`
	- Avoin login endpoint, palauttaa tokenin
- `GET /WeatherForecast/GetSecret`
	- Vaatii policyyn perustuvan admin-oikeuden:
	- `[Authorize(Policy = "RequireAdminRole")]`

## Demo-käyttäjät

- Tavallinen käyttäjä:
	- `username: testuser`
	- `password: testpassword`
- Admin-käyttäjä:
	- `username: admin`
	- `password: adminpassword`

## Käynnistys

```powershell
dotnet restore .\AutentikaatioAutorisaatio.csproj
dotnet run
```

Swagger avautuu yleensä osoitteeseen:

- `https://localhost:7207/swagger`
tai
- `http://localhost:5268/swagger`

## Testaa autentikointi Swaggerissa

1. Kutsu `POST /WeatherForecast/login` ja hae token.
2. Paina Swaggerissa `Authorize`.
3. Syötä: `Bearer <token>`
4. Testaa endpointit:
	 - `OpenGet` toimii ilman tokenia.
	 - `AuthGet` vaatii tokenin.
	 - `GetSecret` toimii vain admin-tokenilla.

## Odotetut vastaukset

- `GetSecret` ilman tokenia -> `401 Unauthorized`
- `GetSecret` user-tokenilla -> `403 Forbidden`
- `GetSecret` admin-tokenilla -> `200 OK`

## Huomio tuotantoon

Tässä demossa salainen avain, issuer ja audience ovat kovakoodattuja esimerkin vuoksi.
Tuotantokäytössä ne tulee siirtää `appsettings.json`-tiedostoon tai salaisuuksien hallintaan (esim. User Secrets / Key Vault).
