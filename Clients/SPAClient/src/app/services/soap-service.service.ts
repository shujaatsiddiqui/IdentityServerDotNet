import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SoapService {
  private soapUrl = 'http://localhost:56551/Service1.svc'; // Replace with your SOAP service URL

  constructor(private http: HttpClient) {}

  callSoapService(requestXml: string): Observable<any> {
   const headers = new HttpHeaders({
      'Content-Type': 'text/xml; charset=utf-8',
      'Accept': 'application/xml',
      'SOAPAction': 'http://tempuri.org/IService1/GetData'
    });

    return this.http.post(this.soapUrl, requestXml, { headers, responseType: 'text' });
  }
}
