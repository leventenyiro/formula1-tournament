import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, tap, throwError } from 'rxjs';
import { environment } from '../../environments/environment'
import { Season } from '../models/season';
import { Driver } from 'app/models/driver';
import { Result } from 'app/models/result';
import { Team } from 'app/models/team';
import { Race } from 'app/models/race';

@Injectable({
  providedIn: 'root'
})
export class SeasonService {

  constructor(private http: HttpClient) { }

  getSeasons(): Observable<Season[]> {
    let headers = new HttpHeaders().set('content-type', 'application/json').set('Access-Control-Allow-Origin', '*')
    return this.http.get<Season[]>(
      `${environment.backendUrl}/season`,
        {
            headers: headers
        }
    ).pipe(
        tap(data => JSON.stringify(data)),
        catchError((error: HttpErrorResponse) => {
          return throwError(() => new Error(error.error));
        })
    )
  }

  getSeasonsByUser(documentCookie: string): Observable<Season[]> {
    const bearerToken = documentCookie.split("session=")[1].split(";")[0];
    let headers = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*')
    .set('Authorization', `Bearer ${bearerToken}`);

    return this.http.get<Season[]>(
      `${environment.backendUrl}/season/user`,
        {
            headers: headers
        }
    ).pipe(
        tap(data => JSON.stringify(data)),
        catchError((error: HttpErrorResponse) => {
          return throwError(() => new Error(error.error));
        })
    )
  }

  createSeason(season: Season, documentCookie: string) {
    const bearerToken = documentCookie.split("session=")[1].split(";")[0];
    let headers = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*')
    .set('Authorization', `Bearer ${bearerToken}`);

    return this.http.post(
      `${environment.backendUrl}/season`,
      {
        "name": season.name,
        "description": season.description
      },
      {
        headers: headers,
        responseType: 'text'
      }
    ).pipe(
      tap(data => data),
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error));
      })
    )
  }

  getSeason(id: string) {
    let headers = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*')
    return this.http.get<Season>(
      `${environment.backendUrl}/season/${id}`,
        {
            headers: headers
        }
    ).pipe(
        tap(data => JSON.stringify(data)),
        catchError((error: HttpErrorResponse) => {
          return throwError(() => new Error(error.error));
        })
    )
  }

  deleteDriver(id: string) {
    const bearerToken = document.cookie.split("session=")[1].split(";")[0];
    let headers = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*')
    .set('Authorization', `Bearer ${bearerToken}`);

    return this.http.delete(
      `${environment.backendUrl}/driver/${id}`,
      {
        headers: headers
      }
    ).pipe(
      tap(data => data),
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error));
      })
    )
  }

  deleteResult(id: string) {
    const bearerToken = document.cookie.split("session=")[1].split(";")[0];
    let headers = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*')
    .set('Authorization', `Bearer ${bearerToken}`);

    return this.http.delete(
      `${environment.backendUrl}/result/${id}`,
      {
        headers: headers
      }
    ).pipe(
      tap(data => data),
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error));
      })
    )
  }

  deleteTeam(id: string) {
    const bearerToken = document.cookie.split("session=")[1].split(";")[0];
    let headers = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*')
    .set('Authorization', `Bearer ${bearerToken}`);

    return this.http.delete(
      `${environment.backendUrl}/team/${id}`,
      {
        headers: headers
      }
    ).pipe(
      tap(data => data),
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error));
      })
    )
  }

  deleteRace(id: string) {
    const bearerToken = document.cookie.split("session=")[1].split(";")[0];
    let headers = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*')
    .set('Authorization', `Bearer ${bearerToken}`);

    return this.http.delete(
      `${environment.backendUrl}/race/${id}`,
      {
        headers: headers
      }
    ).pipe(
      tap(data => data),
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error));
      })
    )
  }

  deletePermission(id: string) {
    const bearerToken = document.cookie.split("session=")[1].split(";")[0];
    let headers = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*')
    .set('Authorization', `Bearer ${bearerToken}`);

    return this.http.delete(
      `${environment.backendUrl}/permission/${id}`,
      {
        headers: headers
      }
    ).pipe(
      tap(data => data),
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error));
      })
    )
  }

  updatePermission(id: string) {
    const bearerToken = document.cookie.split("session=")[1].split(";")[0];
    let headers = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*')
    .set('Authorization', `Bearer ${bearerToken}`);

    return this.http.put(
      `${environment.backendUrl}/permission/${id}`, null,
      {
        headers: headers
      }
    ).pipe(
      tap(data => data),
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error));
      })
    )
  }

  createDriver(driver: Driver, seasonId: string) {
    const bearerToken = document.cookie.split("session=")[1].split(";")[0];
    let headers = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*')
    .set('Authorization', `Bearer ${bearerToken}`);

    return this.http.post(
      `${environment.backendUrl}/season/${seasonId}/driver`,
      {
        "name": driver.name,
        "realName": driver.realName,
        "number": driver.number,
        "actualTeamId": driver.actualTeamId
      },
      {
        headers: headers,
        responseType: 'text'
      }
    ).pipe(
      tap(data => data),
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error));
      })
    )
  }

  updateDriver(id: string, driver: Driver) {
    const bearerToken = document.cookie.split("session=")[1].split(";")[0];
    let headers = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*')
    .set('Authorization', `Bearer ${bearerToken}`);

    return this.http.put(
      `${environment.backendUrl}/driver/${id}`,
      {
        "name": driver.name,
        "realName": driver.realName,
        "number": driver.number,
        "actualTeamId": driver.actualTeamId
      },
      {
        headers: headers,
        responseType: 'text'
      }
    ).pipe(
      tap(data => data),
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error));
      })
    )
  }

  createResult(result: Result) {
    const bearerToken = document.cookie.split("session=")[1].split(";")[0];
    let headers = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*')
    .set('Authorization', `Bearer ${bearerToken}`);

    return this.http.post(
      `${environment.backendUrl}/race/${result.raceId}/result`,
      {
        "type": result.type,
        "position": result.position,
        "point": result.point,
        "driverId": result.driverId,
        "teamId": result.teamId
      },
      {
        headers: headers,
        responseType: 'text'
      }
    ).pipe(
      tap(data => data),
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error));
      })
    )
  }

  updateResult(id: string, result: Result) {
    const bearerToken = document.cookie.split("session=")[1].split(";")[0];
    let headers = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*')
    .set('Authorization', `Bearer ${bearerToken}`);  
    
    return this.http.put(
      `${environment.backendUrl}/result/${id}`,
      {
        "type": result.type,
        "position": result.position,
        "point": result.point,
        "driverId": result.driverId,
        "teamId": result.teamId
      },
      {
        headers: headers,
        responseType: 'text'
      }
    ).pipe(
      tap(data => data),
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error));
      })
    )
  }

  createTeam(team: Team, seasonId: string) {
    const bearerToken = document.cookie.split("session=")[1].split(";")[0];
    let headers = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*')
    .set('Authorization', `Bearer ${bearerToken}`);

    return this.http.post(
      `${environment.backendUrl}/season/${seasonId}/team`,
      {
        "name": team.name,
        "color": team.color
      },
      {
        headers: headers,
        responseType: 'text'
      }
    ).pipe(
      tap(data => data),
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error));
      })
    )
  }

  updateTeam(id: string, team: Team) {
    const bearerToken = document.cookie.split("session=")[1].split(";")[0];
    let headers = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*')
    .set('Authorization', `Bearer ${bearerToken}`);

    return this.http.put(
      `${environment.backendUrl}/team/${id}`,
      {
        "name": team.name,
        "color": team.color
      },
      {
        headers: headers,
        responseType: 'text'
      }
    ).pipe(
      tap(data => data),
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error));
      })
    )
  }

  createRace(race: Race, seasonId: string) {
    const bearerToken = document.cookie.split("session=")[1].split(";")[0];
    let headers = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*')
    .set('Authorization', `Bearer ${bearerToken}`);

    return this.http.post(
      `${environment.backendUrl}/season/${seasonId}/race`,
      {
        "name": race.name,
        "dateTime": race.dateTime
      },
      {
        headers: headers,
        responseType: 'text'
      }
    ).pipe(
      tap(data => data),
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error));
      })
    )
  }

  updateRace(id: string, race: Race) {
    const bearerToken = document.cookie.split("session=")[1].split(";")[0];
    let headers = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*')
    .set('Authorization', `Bearer ${bearerToken}`);

    return this.http.put(
      `${environment.backendUrl}/race/${id}`,
      {
        "name": race.name,
        "dateTime": race.dateTime
      },
      {
        headers: headers,
        responseType: 'text'
      }
    ).pipe(
      tap(data => data),
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error));
      })
    )
  }
}
