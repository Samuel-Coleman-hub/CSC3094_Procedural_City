
# Procedurally Generating Urban Enviroments for Video Games

This is the project file for my final year project, in which I create a customisable procedural city generation tool.



## Acknowledgements

 - [L-System Road Generation Tutorials](https://www.youtube.com/watch?v=umedtEzrpvU&list=PLcRSafycjWFcbaI8Dzab9sTy5cAQzLHoy&index=1&t=12s)
 - [Procedural Map Generation](https://www.youtube.com/watch?v=3G5d8ob_Lfo)



## Author

- [@Samuel-Coleman-hub](https://github.com/Samuel-Coleman-hub)


## How to use

To use the scripts created in this project you will first need to create the following six GameObjects in a Unity Project and attach their respective scripts.

- GridSpawner
- RoadHelper
- RoadVisualiser
- L-System
- Building Generator
- CityManager


### RoadHelper
Pass the RoadHelper script road tile assets to use for the road generation, these assets should each by of a size 1 x 1 world space units.

### CityManager
Adjust CityManager parameters in the inspector window, to change the size of the grid.

The city generation works through the generation of different city zones, with each city zone handling how content is generated within it.

#### Zone Specific Settings

Within CityManager you can add Zones to the "City Zones" array.

Within each zone you can adjust the length of the road it will generate, the L-System rule that it will follow, these can be created as ScriptableObjects within City/Rule.

Parameters can also be adjusted for building generation, changing the size of buildings generated and adjusting how height and width of a building changes the closer it is to the center, by manipulating the AnimationCurves.

Modular assets used for the generation of buildings can be specified in the Base, Middle and top units, to denote the pieces that can be spawned at each tier of the building.

Finally Misc object prefabs can be added and Misc Density can be used to adjust the relative objects density in the zone.

#### Generate City
To generate a city from these requirements simply run the Unity project.




## Demo

The sample scene found in Assets/Scenes/ExampleCity provides an enviroment with the tool setup and ready to use.

Manipulate the variables on the CityManager component of the CityManager GameObject and run the game to generate a city enviroment.

