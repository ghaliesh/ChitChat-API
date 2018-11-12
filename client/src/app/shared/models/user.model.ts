export interface IUser {
  name: string;
  banned: boolean;
  email: string;
  id: string;
  photo: {
    imgURL: string;
  };
  userName: string;
  warns: number;
}
