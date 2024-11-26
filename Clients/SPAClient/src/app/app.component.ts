import { Component, OnInit } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { authConfig } from './Config/auth-config';
import { SoapService } from './services/soap-service.service';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  accessToken: string | null = null;
  private apiUrl = 'https://localhost:7256/WeatherForecast';  // Your Web API URL
  weatherData!: any[];

  constructor(private oauthService: OAuthService, private soapService: SoapService,
    private http: HttpClient  ) {}

  ngOnInit(): void {
    this.configureOAuth();
    this.oauthService.loadDiscoveryDocumentAndTryLogin().then(() => {
      if (this.oauthService.hasValidAccessToken()) {
        this.accessToken = this.oauthService.getAccessToken();
      }
    });
  }

  configureOAuth(): void {
    this.oauthService.configure(authConfig);
    this.oauthService.setupAutomaticSilentRefresh();
  }

  login(): void {
    this.oauthService.initLoginFlow();
  }

  logout(): void {
    this.oauthService.logOut();
  }

  callSoap() {
    const requestXml = '<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ns="http://tempuri.org/"><soap:Body><ns:GetData><ns:value>123</ns:value></ns:GetData></soap:Body></soap:Envelope>';

    this.soapService.callSoapService(requestXml).subscribe(
      (response) => {
        alert('SOAP Response: ' + response);
        // Parse the XML response if needed
      },
      (error) => {
        alert('SOAP Error: '+ error);
      }
    );
  }

  getWeatherForecast(): void {
    this.http.get<any[]>(this.apiUrl)
      .subscribe(
        (data) => {
          this.weatherData = data;  // Store the data in a variable
          console.log(data);  // Log the data to console
        },
        (error) => {
          console.error('Error fetching weather data', error);  // Handle errors
        }
      );
  }
}
