import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationGuard {
  canActivate(): boolean {
    return true;
  }
}
