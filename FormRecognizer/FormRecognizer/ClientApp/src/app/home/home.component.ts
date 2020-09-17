import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { SimpleReceiptViewModel } from '../models/SimpleReceiptViewModel';
import { BehaviorSubject } from 'rxjs';

export interface PeriodicElement {
  name: string;
  position: number;
  weight: number;
  symbol: string;
}

const ELEMENT_DATA: PeriodicElement[] = [
  { position: 1, name: 'Hydrogen', weight: 1.0079, symbol: 'H' },
  { position: 2, name: 'Helium', weight: 4.0026, symbol: 'He' },
  { position: 3, name: 'Lithium', weight: 6.941, symbol: 'Li' },
  { position: 4, name: 'Beryllium', weight: 9.0122, symbol: 'Be' },
  { position: 5, name: 'Boron', weight: 10.811, symbol: 'B' },
  { position: 6, name: 'Carbon', weight: 12.0107, symbol: 'C' },
  { position: 7, name: 'Nitrogen', weight: 14.0067, symbol: 'N' },
  { position: 8, name: 'Oxygen', weight: 15.9994, symbol: 'O' },
  { position: 9, name: 'Fluorine', weight: 18.9984, symbol: 'F' },
  { position: 10, name: 'Neon', weight: 20.1797, symbol: 'Ne' },
];

@Component({
  selector: 'app-home',
  styleUrls: ['home.component.css'],
  templateUrl: './home.component.html',
})

export class HomeComponent implements OnInit {

  displayedColumns: string[] = ['position', 'name', 'weight', 'symbol'];
  displayedColumns2: string[] = ['merchantName', 'merchantAddress', 'totalAmount', 'blobUrl' ];
  dataSource = ELEMENT_DATA;
  public dataSource2 = new BehaviorSubject([]);
  public data : SimpleReceiptViewModel[] = [];

  constructor(private changeDetectorRefs: ChangeDetectorRef) {

  }
  ngOnInit(): void {
    const connection = new signalR.HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.Information)
      .withUrl("https://localhost:44393/pushhub")
      .build();

    connection.start().then(function () {
      console.log('SignalR Connected!');
    }).catch(function (err) {
      return console.error(err.toString());
    });

    connection.on("test", (username: string, message: SimpleReceiptViewModel) => {
      console.log(username);
      console.log(message);
      this.data.push(message);
      this.dataSource2.next(this.data);
      console.log(this.data);
    });  
  }
}
