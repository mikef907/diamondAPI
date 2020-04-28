import { IUserModel } from './i-user-model';

export class User implements IUserModel {
    id: string;
    firstName: string;
    lastName: string;
    password: string;
    email: string;

    constructor(data: IUserModel) {
        Object.keys(data).forEach(key => this[key] = data[key]);
    }
}