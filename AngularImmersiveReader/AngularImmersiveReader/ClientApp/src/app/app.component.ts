import { Component, Inject } from '@angular/core';
import { launchAsync } from '@microsoft/immersive-reader-sdk';
import { HttpClient } from '@angular/common/http';
import { Observable, Subscription } from 'rxjs';
import { map, startWith } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  title = 'app';

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
  }

  getToken(): Observable<any> {
    return this.http
      .get(this.baseUrl + 'ai/get-token-and-subdomain')
      .pipe(
        map((token: any) => {
          return token;
        })
      );
  }

  LaunchImmersive() {

    this.getToken().subscribe(token => {
      //read the content of the pages
      const htmlContent = document.getElementById('body-content').innerHTML;

      //prepare content
      const content = {
        title: 'Immersive Reader',
        chunks: [{
          content: htmlContent,
          mimeType: 'text/html'
        }]
      };

      //prepare options
      const options = {
        //readAloudOptions: {
        //  autoplay: true,
        //  voice: 'male',
        //  speed: 0.5
        //},
        //translationOptions: {
        //  language: 'fr-FR',
        //  autoEnableDocumentTranslation: true,
        //  autoEnableWordTranslation: true
        //}
      };

      launchAsync(token.token, token.subdomain, content, options);
    })
  }
}
