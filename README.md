Unity3D code configure setting readme

1>	Open unity project
Age Of Empire:
AgeOfEmpire\Client\AgeOfEmpire\Assets\Scene\loginscene.unity  this file is used to open the Unity Project


2> Path
AgeOfEmpire\Client\AgeOfEmpire\Assets\Resources is the resource path
AgeOfEmpire\Client\AgeOfEmpire\Assets\Script is the script path

3>	About the version

 ![image](https://user-images.githubusercontent.com/93575862/236138445-a288f9f1-5f86-4974-a76d-cca87e4b4538.png)


When you need to debug in your PC side. You need to set the variable VERSION to be VERSION_INNER.
 
![image](https://user-images.githubusercontent.com/93575862/236138467-c81b063e-2cc2-49c4-88d4-2a5eb06694f9.png)


When you need to output the APP version. You need to set variable VERSION to be VERSION_APPSTORE
 
![image](https://user-images.githubusercontent.com/93575862/236138476-ec01fd95-dd45-4ed3-bae4-a920fecb1b86.png)



4>	Server update path setting

The variable updateRouter means the client request the configure file path using php request.
It is composed with the IP+server file path.
Eg. 192.168.1.10 is the IP of our server.  hwl/update is the server update path.
![image](https://user-images.githubusercontent.com/93575862/236138495-6c1f816b-0c51-48d6-a4f0-06b4e0af3575.png)

 
 ------------------------------------------------------------------------------------------------------------------
----------------------------------------**IOS BUILD:**-----------------------------------------------------------

1>  During the Xcode building,You need to copy the AppleCmn directory to the same place as the AgeofEmpire  Unity3d output directory goes. Refer to the piture below:

![image](https://user-images.githubusercontent.com/93575862/236237273-b920690b-6e68-4bd2-ac76-1b898f0be60c.png)

2>  Add the AppleCmn contains file to the Xcode project. Shown as below:
![image](https://user-images.githubusercontent.com/93575862/236237367-25d69128-c810-4706-83bf-2c61bf4fd46e.png)

3>  Add necessary frameworks to the Xcode project. Shown as below:

![image](https://user-images.githubusercontent.com/93575862/236237436-319cd540-ba03-4e06-80c3-ebc7ef510904.png)

4>IAP signature

![image](https://user-images.githubusercontent.com/93575862/236237508-1e61d651-9256-4dba-91ff-a2e303964806.png)

In the file APIApple.mm you can see the signature.
“@com.AgeofEmpire.diamonds.%d”  modify this to be yours.

![image](https://user-images.githubusercontent.com/93575862/236237571-4da6e11c-32d9-4d74-a4ee-1a7c9515a04d.png)


