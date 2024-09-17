# DHGSystems.FileSortingWithLimitedMemory

This app is developed to meet the this criteria:
   - be able to process  1GB - 100 GB file with limited memory <16 GB
   - solution will be run on multicore processor like Intel i9 9900K and fast SSD disks
   - one file is processed at time
   - data can be completly random 
       - no presort of first char - this could not work for file with all rows on letter A
       - no string dictionary or string intern
   - this sulution is mostly limited by disk speed, so clening the GC has small impact on performance
   - this app divide file every 1500000 lines (max 1024 Unicode characters) program need  around 32 GB of memory, if failing the configuration should be changed
      // 1500000 * 1024B *2  = 3 GB * 4 workers = 8 GB of memory * 3.5 OrderBy mesh at one time = 28 GB of memory 
       

How it works:
  - the big file is read by one worker and divided in smaller sorted files ( 4 Task based worker)
  - small files are being merge (separate task ) and sorted when we get the specified number of smile files, merged files are created
  - small files and merged files are merged in repeat, for each set of files a new task is started
  - files are merged until final file is created

What could make solutions faster:
  - separate disk for files
  - more cores
  - set proper proportion off parameters
      if disk are 100% in use -> make a bunch of files to merge bigger.
   

Results:
  - sorting the File size 103385 MB.    is taking around 17.21 minute (Time 1,033,585.0 ms) disk speed is limit. Max memory usage  <9 GB  - max line length 1024 chars