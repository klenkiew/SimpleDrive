export class File {
  id: string;
  name: string;
  size: number;
  lastModified: Date;

  constructor(id: string, name: string, size: number, lastModified: Date) {
    this.id = id;
    this.name = name;
    this.size = size;
    this.lastModified = lastModified;
  }
}
