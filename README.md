# Représentations géométriques et subdivision de Mesh

## Equipe

Lors de notre 4ème d'études à l'école d'ingénieurs ESIEE Paris, nous avons réalisé un project de modélisation 3D. La composition de l'équipe est la suivante :

> Ewen BOUQUET ;

> Fabien COURTOIS ;

> Victor HUGER ;

## Contenu du projet

Notre projet comprend une unique scène, nommée "MainScene", qui contient l'ensemble des fonctionnalités développées. Les fonctionnalités sont les suivantes :
- Calcul de la distance d’un point à une droite ;
- Calcul de la distance d’un point à un plan ;
- Intersection entre un segment et un plan (plan infini) ;
- Intersection entre un segment et une sphère ;
- Intersection entre un segment et un cylindre (cylindre infini) ;
- Implémentation de l’algorithme de subdivision de surface de Catmull-Clark ;

Lorsqu'on lance le projet Unity, la caméra est par défaut centrée sur le résultat des divisions de Mesh via l'algorithme de Catmull-Clark. Il faut ainsi dézoomer et se déplacer sur la droite (dans la même scène) afin de pouvoir visualiser les intersections entre les formes (immobiles) et le segment (mobile).

## Intersections
L'intersection entre le segment et le plan infini est visible dans la scène lorsque le plan devient rouge et qu'une petite sphère verte apparaît à l'endroit où le segment coupe le plan. 

Notre implémentation du plan possède une limitation, il n'est visible que d'un côté. Il faut donc faire attention à l'endroit depuis lequel on observe cette intersection.

Les deux autres intersections (segment-sphère et segment-cylindre) sont construites de la même manière.

## Catmull-Clark
L'implémentation de l’algorithme de subdivision de surface de Catmull-Clark est illustrée à l'aide de deux formes, un cube et un prisme droit.

Pour choisir la forme à afficher, il faut aller dans l'onglet "Hierarchy" et sélectionner l'objet MeshManager auquel est rattaché le script Mesh Manager. 
Depuis l'inspecteur on peut choisir la forme souhaitée, "Cube" ou "Straight Prism". D'autres paramètres du script peuvent également être modifiés, notamment "Subdivision Nb" pour définir nombre de subdivision. 

Cependant, il faut relancer la simulation (le programme) pour que les changements soient effectifs. En effet, aucune modification pendant le play mode n'est possible.
Dans le dossier "Screenshots" qui se trouve à la racine du projet, nous avons mis des captures d'écran pour illustrer les différentes fonctionnalités implémentées.

## Bugs
L'unique bug que nous avons à rapporter est la présence de trous dans les formes lorsque la subdivision de surface de Catmull-Clark est exécutée plus de trois fois.

## Travail en équipe
Pour mener à bien ce projet, nous avons décidé de travailler le plus possible ensemble. 
Ewen et Fabien se sont plus concentrés sur le code, tandis que Victor s'est plus penché sur la partie mathématique des implémentations. 

Cependant, nous tous touché à tout dans le projet, et nous sommes beaucoup entraidés, afin que chacun puisse progresser sur les domaines liés au projet.
