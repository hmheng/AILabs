import { Component, Inject } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import { BookingModel } from '../../models/booking.model';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {

  bookings: BookingModel[] = [];
  private connection = new signalR.HubConnectionBuilder()
    .withUrl("/pushhub")
    .build();

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.connection.on("CreateProduct", (message) => {
      console.log('CreateProduct', message);
      this.getAllBookings();
    });
    this.connection.start().catch(err => document.write(err));

    this.getAllBookings();
  }

  getAllBookings() {
    this.http
      .get(this.baseUrl +`api/booking/get-all-bookings`, { observe: 'response'})
      .pipe(
        map((response: HttpResponse<BookingModel[]>) => {
          if (response) {
            console.log('getAllBookings', response);
            this.bookings = response.body;
            console.log('getAllBookings', this.bookings);
            return response.body;
          }
        })
    ).subscribe();
  }
}
