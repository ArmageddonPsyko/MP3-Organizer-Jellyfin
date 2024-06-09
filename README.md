#########MP3-Organizer-Jellyfin##########


Audio files will be sorted by artist and album. If there are multiple artists in the metadata, commas will be replaced by semicolons (Jellyfin does not accept commas for artist separation, resulting in incorrect metadata in the Jellyfin database).

Folder structure:

    Artist folder
        Album folder
            Album tracks

Only the first artist is used for the artist folder name, as Jellyfin will automatically add album information via metadata.

**Valid ID3 information is required for this to work.**

Original Code by 
[bluser86]([https://pages.github.com/](https://github.com/bluser86/MP3-Organizer
) /
Code modified for Jellyfin by ArmageddonPsyko
