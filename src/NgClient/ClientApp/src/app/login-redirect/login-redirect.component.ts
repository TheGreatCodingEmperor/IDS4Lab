import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { url } from 'inspector';

@Component({
  selector: 'app-login-redirect',
  templateUrl: './login-redirect.component.html',
  styleUrls: ['./login-redirect.component.css']
})
export class LoginRedirectComponent implements OnInit {
  private ids4Server = "https://localhost:5001";

  constructor(private http: HttpClient) { }

  ngOnInit() {
    let headers = new HttpHeaders();
    headers.append("content-type", "application/x-www-form-urlencoded");
    let url = new URL(`${this.ids4Server}/connect/token`);
    var formData = new FormData();
    formData.append("grant_type", "password");
    formData.append("client_id", "angular");
    formData.append("scope", "api1");
    formData.append("username", "alice");
    formData.append("password", "alice");

    this.http.post(url.toString(), formData, { headers }).subscribe(res => { console.log(res) });
  }

}
