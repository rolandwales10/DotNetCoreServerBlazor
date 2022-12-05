# Farmshare admin readme

Created as a .net core 6.0 application using Studio 2022.
Project type is a .net core Blazor Server app.

Business purpose: allow farmshare administrators to allocate shares to farmers with a global view
of all farms.  When shares are updated, related fields are also updated on the form with spreadsheet
like functionality, giving immediate feedback to things like remaining shares available.

Software Features:
- When an error is trapped, logs all inner exceptions.  See Utilities.Error
- When data is saved, logs exceptions.  See BusinessAreaLayer.UnitOfWork
- Role based Active Directory security. See Utilities.Authorization and its use in pages
- Reading configuration from AppConfig.json in program.cs and also in a Razor page
- Creating services for reuse in the application.  See Builder.AddServices in program.cs.
- Blazor is being used to create an editable grid form in ShareAllocations.Razor.  Binding 
  fields in the DOM to a data structure makes this type of form clean and relatively simple
  to work with.