import { signalRConnection } from "./connection";

const changeAccount = (index, oldIndex) => {
	if (index !== -1) {
		signalRConnection.invoke("AddGroup", index);
	}
	if (oldIndex !== -1) {
		signalRConnection.invoke("RemoveGroup", oldIndex);
	}
};

export { changeAccount };
