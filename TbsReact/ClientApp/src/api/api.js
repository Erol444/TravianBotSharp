/** /settings/${type}/ **/
export const CHROME_SETTING = "chrome";
export const ACTIVITY_SETTING = "activity";
export const DISCORD_WEBHOOK_SETTING = "discordwebhook";
export const HERO_SETTING = "hero";
export const QUEST_SETTING = "quest";

export { getVillages } from "./Village";
export { getLogData, getTaskList } from "./Debug";
export { checkUrlDiscordWebhook } from "./Discord";
export { getSetting, setSetting } from "./Setting";

export {
	getAccounts,
	getAccount,
	addAccount,
	editAccount,
	deleteAccount,
} from "./Accounts/Account";
export { getAccesses } from "./Accounts/Access";
export { login, logout, getStatus } from "./Accounts/Driver";
