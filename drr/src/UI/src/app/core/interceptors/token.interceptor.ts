import { Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class TokenInterceptor {
    intercept(req: any, next: any) {
        return next.handle(req);
    }
}