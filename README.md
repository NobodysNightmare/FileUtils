FileUtils
=========

This repository contains various C# projects that I needed at different times for different purposes.

ComparePathCLI
--------------

This is used to compare directory contents, I use it to keep my media-libraries synced.
There is currently no automagic syncing, but I might want to add that at some time.

DriveKeepAlive
--------------

I have an annoying hard-drive, that spins down every few seconds. It will only keep spinning (or spin up, taking seconds)
when there is activity. So far I found no firmware-upgrades fot that issue.
This tool will write a single byte to the drive in a regular interval, this prevents the drive from spinning down.

To keep things green the tool will stop writing when there is no user-interaction with the machine for a certain amount of time.
This means the drive will spin down when I am not at my computer.

FileBloater
-----------

This convenient little tool will create a file in its own directory and increase its size until an error occures.
This can be helpful if your hard drive encounters bad sectors that it needs to be written to in order to try to re-allocate them.
Doing it this way is quite safe, as it will only consume unallocated space, so it is not possible that you will erase existing file contents (except for hard-drive-failure due to write-activity). However, you will also only be able to fix sectors that don't belong to allocated files.