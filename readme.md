# Farmshare admin readme

Created as a .net core 6.0 application using Studio 2022.
Project type is a .net core Blazor Server app.

Software Features:
- When an error is trapped, logs all inner exceptions.  See Utilities.Error
- When data is saved, logs exceptions.  See BusinessAreaLayer.UnitOfWork
- Role based Active Directory security. See Pages.BasePage
- Reading configuration from AppConfig.json in program.cs and also in a Razor page
