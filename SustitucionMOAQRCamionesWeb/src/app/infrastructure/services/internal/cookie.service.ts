import { Injectable } from "@angular/core"

@Injectable({
  providedIn: "root",
})
export class CookieService {
  setCookie(name: string, value: string, days = 365): void {
    const date = new Date()
    date.setTime(date.getTime() + days * 24 * 60 * 60 * 1000)
    const expires = `expires=${date.toUTCString()}`
    document.cookie = `${name}=${value};${expires};path=/`
  }

  setSessionCookie(name: string, value: string, minutes: number): void {
    const maxAge = minutes * 60; // 5 minutes = 300 seconds
    document.cookie = `${name}=${value};max-age=${maxAge};path=/`;
  }

  getCookie(name: string): string | null {
    const nameEQ = `${name}=`
    const ca = document.cookie.split(";")
    for (let i = 0; i < ca.length; i++) {
      let c = ca[i]
      while (c.charAt(0) === " ") c = c.substring(1, c.length)
      if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length)
    }
    return null
  }

  deleteCookie(name: string): void {
    document.cookie = `${name}=;expires=Thu, 01 Jan 1970 00:00:00 UTC;path=/;`
  }
}