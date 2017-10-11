# Microsoft Experiences 2017 - Démo Azure Service Fabric et hyper scaling
Code source de la démo hyperscaling service fabric lors de la session retour d'experience Adents
https://experiences17.microsoft.fr/session/89079476-3e70-e711-80c2-000d3a210b7f

Les slides sont accessibles ici : https://www.slideshare.net/flyingoverclouds/microsoft-experiences2017-architecture-microservices-comment-adents-a-transform-son-business-en-dployant-une-application-pense-pour-le-cloud 
    

## Todo :
  les 4 modules applicatifs de la démo (le front end web, le service stateless, les 2 exe ligne de commande) ont besoin des credentials des 2 queues utilisées par la démo.
  
  Il faut ajouter ces credentials (nom et key du storage account) dans les fichiers suivants 
  (bien entendu ... vous pouvez aussi externaliser les config :) )
  
  * HyperScaleDemoExperience2017/DiscountCalculatorSvc/DiscountCalculatorSvc.cs
  * HyperScaleDemoExperience2017/OrdersDiscountedReader/Program.cs
  * HyperScaleDemoExperience2017/OrdersPublisher/Program.cs
  * HyperScaleDemoExperience2017/WebFrontEnd\Controllers/HomeController.cs
