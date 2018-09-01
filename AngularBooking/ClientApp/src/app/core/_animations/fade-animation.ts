import { trigger, transition, style, animate, query } from "@angular/animations";

export const fadeAnimation =
  trigger("fadeAnimation", [
    transition('* => *', [
      query(':enter', [
        style({ opacity: 0, position: 'fixed', width: '100%', height: '100%', display: 'block' }),
      ], { optional: true }),
      query(':leave', [
        style({ opacity: 1, position: 'fixed', width: '100%', height: '100%', display: 'block'}),
        animate('.3s', style({ opacity: 0 }))
      ], { optional: true }),
      query(':enter', [
        style({ opacity: 0, position: 'fixed', height: '100%', width: '100%', display: 'block' }),
        animate('.3s', style({ opacity: 1 }))
      ], { optional: true })
    ])
  ]);
