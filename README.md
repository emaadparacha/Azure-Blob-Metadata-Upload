# Azure Blob Storage Upload with Metadata

This project uploads a file to Azure Blob Storage and extracts text from the file, parses it, and applies it as metadata in Blob Storage.

A demo SRT file is provided, to test text extraction and metadata upload.

Please change the StorageConnectionString in App.config in order to upload to the right storage account.
Also please add an API Key for Azure Maps in Program.cs in "string crossurl" to connect to Azure Maps to get cross street info.

This demo uploads the SRT file as "DemoUpload" to container "democontainer". These settings can also be changed.

The date and time are extracted from the 4th line of the SRT file, and latitude and longitude are extracted from the 5th line. The date is also converted from numeric form to text form. The coordinates are fed into Azure Maps, and a JSON file is read where the corresponding cross street is calculated. At the end, metadata is applied to the blob and it is uploaded to Blob Storage.


