import { HubConnectionBuilder } from "@microsoft/signalr";

let signalRConnection;

const initConnection = () => {
  signalRConnection = new HubConnectionBuilder().withUrl("/live").build();
};

export { signalRConnection, initConnection };
