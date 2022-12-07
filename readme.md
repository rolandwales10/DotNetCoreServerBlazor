# Farmshare admin readme

Created as a .net core 6.0 application using Studio 2022.
Project type is a .net core Blazor Server app.

Business purpose: allow farmshare administrators to allocate shares to farmers with a global view
of all farms.  When shares are updated, related fields are also updated on the form with spreadsheet
like functionality, giving immediate feedback to things like remaining shares available.

Software Features:
- Active Directory authentication: Enabled in launchSettings.json and Program.cs.
- Role based Active Directory authorization: See Utilities.Authorization and its use in pages
- When an error is trapped, logs all inner exceptions.  See Utilities.Logging
- For the use of direct SQL without LINQ, use a database connection.  Open the database connection once per http request
  and reuse it in the Logging service.  This is possible with the use of scoped services.
- When data is saved, logs exceptions.  See BusinessAreaLayer.UnitOfWork
- Reading configuration from AppConfig.json in program.cs and also in a Razor page
- Creating services for reuse in the application.  See Builder.AddServices in program.cs.
- Blazor is being used to create an editable grid form in ShareAllocations.Razor.  Binding 
  fields in the DOM to a data structure makes this type of form clean and relatively simple
  to work with.