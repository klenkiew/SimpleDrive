import {User} from "./user";

export class FileLock
{
  constructor(public readonly isLockPresent: boolean, public readonly lockOwner?: User) {
  }
}
