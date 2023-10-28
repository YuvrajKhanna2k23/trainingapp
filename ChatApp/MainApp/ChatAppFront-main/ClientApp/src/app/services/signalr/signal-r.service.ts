import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr'; // import signalR

@Injectable({
  providedIn: 'root'
})
export class SignalRService {

  hubConnection : signalR.HubConnection
  hubUrl :string =  "https://localhost:5050/ChatHub";
  startConnection = (userName : string) =>{
    this.hubConnection = new signalR.HubConnectionBuilder()
    .withUrl(this.hubUrl,{
      skipNegotiation:true,
      transport : signalR.HttpTransportType.WebSockets
    })
    .withAutomaticReconnect()
    .build();

    this.hubConnection.start()
    .then(()=>{

      console.log('hub Connection Started ' + userName);

      this.hubConnection.invoke("ConnectDone",userName)
      .catch((error)=>{console.log(error)});

    }).catch(error=>{console.log(error)})
  }
}
