import {User} from "./user";

export class File {

  constructor(public readonly id: string, public fileName: string, public size: number, public description: string,
              public mimeType: string, public owner: User, public dateCreated: Date, public dateModified: Date) {
  }
}
