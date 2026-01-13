# Data Processing

1. Run CleanSightingsData.py to generate a new CSV called Structured_Bird_Data.csv 
   1. This should hopefully be better formatted than the original
2. Open QGIS
   1. To Download
   2. https://qgis.org/download/
3. Click New Project
4. Click Layer > Add Layer > Add Delimited Text Layer 
5. Select the Cleaned CSV to use
6. Update the projection and make sure the X values are Longitude and Y values are Latitude 
7. Select OK
8. Right-Click the newly generated layer and select Export > Save Feature As
9.  Select GeoJson 
10. Save to the DataProcessing folder 