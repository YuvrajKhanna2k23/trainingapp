import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { SendMessage } from '../models/send-message';

@Injectable({providedIn: 'root'})
export class ChatService {

    constructor(private http : HttpClient) { }
    
    getRecentUsers() {
        return this.http.get(environment.apiUrl + "/chat/recent");
    }

    sendChat(username : string, data : SendMessage) {
        return this.http.post(environment.apiUrl + "/chat/" + username, data);
    }

    getChatWithUser(username : string) {
        return this.http.get(environment.apiUrl + "/chat/" + username);
    }
}