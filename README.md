#########MP3-Organizer-Jellyfin##########


Audio files will be sorted by artist and album. If there are multiple artists in the metadata, commas will be replaced by semicolons (Jellyfin does not accept commas for artist separation, resulting in incorrect metadata in the Jellyfin database).

Folder structure:

    Artist folder
        Album folder
            Album tracks

Only the first artist is used for the artist folder name, as Jellyfin will automatically add album information via metadata.

**Valid ID3 information is required for this to work.**



Known Bugs/Flaws:
-Artist names with commas will also be changed (e.g., Earth, Wind & Fire), but due to the small chance of that happening, you can change the artist folder manually. This will be fixed in later versions.
    
Original Code by bluser86
https://github.com/bluser86/MP3-Organizer

Code modified for Jellyfin by ArmageddonPsyko
