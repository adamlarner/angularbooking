export class CsrfResponse {
  constructor(
    public token: string,
    public tokenName: string
  ) { }
}
