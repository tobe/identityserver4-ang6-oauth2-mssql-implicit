import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { HttpClient } from '../../../../node_modules/@angular/common/http';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-protected-area',
  templateUrl: './protected-area.component.html',
  styleUrls: ['./protected-area.component.css']
})
export class ProtectedAreaComponent implements OnInit {

  constructor(
    private _userService: UserService,
    private _httpClient: HttpClient
  ) { }

  public getUserClaims() {
    return this._userService.getUserClaims();
  }

  ngOnInit() {
    this._httpClient.get(`${environment.apiUrl}/values`).subscribe(
      res => console.log(res),
      err => console.log(err)
    );
  }
}
