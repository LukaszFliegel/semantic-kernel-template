import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, Observable, of } from 'rxjs';
import { MessageRequest } from '../models';
import { environment } from '../../environments/environment';
import {Component} from     '@angular/core';

@Injectable({
    providedIn: 'root',
})
export class AssistantService {
        
    private apiUrl = null;

    constructor(private http: HttpClient) {
        const envApiUrl = (window as any).env?.API_URL;
        this.apiUrl = envApiUrl && envApiUrl !== '$API_URL' ? envApiUrl : environment.apiUrl;
        
        console.info('Using apiUrl=' + this.apiUrl);
     }

    startSession(): Observable<any> {
        return this.http.post(`${this.apiUrl}/start`, {}).pipe(
            catchError(this.handleError)
        );
    }

    sendUserMessage(request: MessageRequest): Observable<any> {
        const formData = new FormData();
        formData.append('ChatId', request.chatId);

        if (request.message) {
            formData.append('Message', request.message ?? '');
        }
        
        if (request.cvFile) {
            formData.append('CvFile', request.cvFile);
        }

        return this.http.post(`${this.apiUrl}/ask`, formData).pipe(
            catchError(this.handleError)
        );
    }

    private handleError(error: HttpErrorResponse): Observable<any> {
        return of({ description: 'Oops! Something went wrong. 😕' });
    }
}
