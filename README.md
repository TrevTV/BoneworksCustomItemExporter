# Boneworks Custom Item Exporter
 A new exporter for Boneworks custom items based on the Beat Saber exporters.

 ## How To Install
 Take the SDK folder in the downloaded zip and merge it with your existing SDK folder

 ## How To Setup (single item)
 NOTE: With this exporter you don't need to create a CustomItems GameObject anymore
 1. On your main item GameObject add a Item Descriptor component
 2. Configure the settings
 
 ## How To Setup (multiple items)
 1. On your parent of your items add a Item Descriptor component
 2. Check the 'Is Multiple Items' box
 3. Under 'Data List' set the amount of items there are
 4. Under each element, set their GameObject and other settings

 ## How to export
 1. Open the export panel by going to Window --> Item Exporter
 2. Under that panel, select 'Export (item name)'
 3. Select the export location and press save
 NOTE: If you get a message about having a CustomItems prefab in the root folder when you don't have one, press Yes, this will be fixed in a later version

