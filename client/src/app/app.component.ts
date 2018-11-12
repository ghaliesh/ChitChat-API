import { IUser } from './shared/models/user.model';
import { Component, ViewChild, ElementRef } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { UserService } from './shared/services/user.service';
import { Observable } from 'rxjs';
import Filter from 'bad-words';

import { ChatService } from './shared/services/chat.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  @ViewChild('chatBox')
  chatBox: ElementRef<HTMLElement>;
  @ViewChild('app')
  app: ElementRef<HTMLElement>;
  @ViewChild('model')
  model: ElementRef<HTMLElement>;
  @ViewChild('modelContent')
  modelContent: ElementRef<HTMLElement>;
  public hubConnection: HubConnection;
  filter;
  public curresntUser$: Observable<IUser>;
  public isAuth$: Observable<Boolean>;
  public formGroup: FormGroup;
  public messages = [];

  constructor(
    private formBuidler: FormBuilder,
    private chatService: ChatService,
    private userService: UserService
  ) {
    this.formGroup = this.formBuidler.group({
      name: this.formBuidler.control('', [
        Validators.required,
        Validators.minLength(2)
      ]),
      email: this.formBuidler.control('', [
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(255)
      ]),
      password: this.formBuidler.control('', [
        Validators.required,
        Validators.minLength(6),
        Validators.maxLength(1024)
      ])
    });
  }

  // tslint:disable-next-line:use-life-cycle-interface
  ngOnInit() {
    this.appInitialState();
    const builder = new HubConnectionBuilder();
    this.hubConnection = builder
      .withUrl('http://localhost:5000/hubs/chat')
      .build();

    this.hubConnection.on('Send', (message, user, date) => {
      const timit = new Date(date).toLocaleTimeString();
      this.messages.push({
        message: message,
        user: user,
        date: timit
      });
      setTimeout(() => {
        // timeout to make sure this run after the message was sent from the server
        const chatBox = this.chatBox.nativeElement;
        chatBox.scrollTop = chatBox.scrollHeight;
        const app = document.querySelector('html');
        app.scrollTop = app.scrollHeight;
        const box = this.app.nativeElement;
        box.scrollTop = app.scrollHeight;
      }, 0);
    });

    this.hubConnection.start();
  }

  openModel(e) {
    this.model.nativeElement.style.transform = 'scale(1)';
    this.modelContent.nativeElement.style.transform = 'scale(1)';
  }

  closeModel() {
    this.model.nativeElement.style.transform = 'scale(0)';
    this.modelContent.nativeElement.style.transform = 'scale(0)';
    this.formGroup.clearValidators();
    this.formGroup.markAsUntouched();
  }

  send(e: KeyboardEvent) {
    if (e.target['value'].trim() === '') {
      return;
    }
    if (+e.keyCode === 13) {
      const message = this.filter.clean(e.target['value']);
      this.hubConnection.invoke(
        'Echo',
        message,
        localStorage.getItem('userId').toString()
      );

      e.target['value'] = '';
    }
  }

  register() {
    const user = this.formGroup.value;
    console.log(user);
    this.userService
      .register(user)
      .subscribe(res => this.handleRegisterSuccess(res));
  }

  logout() {
    this.userService.logout();
  }

  handleRegisterSuccess(res) {
    localStorage.setItem('x-token', res.token);
    localStorage.setItem('userId', res.userId);
    this.userService.isLoggedIn.next(true);
    this.closeModel();
    this.getUser();
  }

  getUser() {
    this.userService.whoAMi().subscribe(res => {
      console.log(res);
      this.userService.currentUser.next(res);
      this.curresntUser$ = this.userService.userIdentity();
    });
  }

  appInitialState() {
    this.messages = Array(20).fill({
      message: 'dummy message',
      user: { name: 'John doe' },
      date: 'somewhere in the past'
    }); // just filling the box with dummy messages
    this.filter = new Filter();
    this.filter.removeWords('god'); // exclude the word god from bad words
    this.isAuth$ = this.userService.isAuthenticated();
    this.getUser();
  }

  get name() {
    return this.formGroup.get('name');
  }
  get email() {
    return this.formGroup.get('email');
  }
  get password() {
    return this.formGroup.get('password');
  }
}
