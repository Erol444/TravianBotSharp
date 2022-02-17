import { Button, Grid, TextField, Typography } from "@mui/material";
import { useForm } from "react-hook-form";
import ContentBox from "../../ContentBox";
import React, { useEffect } from "react";
import { toast } from "react-toastify";
import {
	getSetting,
	setSetting,
	checkUrlDiscordWebhook,
	DISCORD_WEBHOOK_SETTING,
} from "../../../api/api";
import { useSelector } from "react-redux";
import MUISwitch from "../../ref/MUISwitch";

const DiscordSettings = () => {
	const {
		register,
		handleSubmit,
		setValue,
		getValues,
		formState: { errors },
		setError,
		control,
	} = useForm();
	const account = useSelector((state) => state.account.info.id);

	useEffect(() => {
		if (account !== -1) {
			getSetting(account, DISCORD_WEBHOOK_SETTING).then((data) => {
				const { isActive, isOnlineMsg, urlWebhook } = data;
				setValue("isActive", isActive);
				setValue("isOnlineMsg", isOnlineMsg);
				setValue("urlWebhook", urlWebhook);
			});
		}
	}, [account, setValue]);

	const onSubmit = async (data) => {
		const isUrlValid = await checkUrl(getValues("url"), false);
		if (!isUrlValid) return;
		setSetting(account, DISCORD_WEBHOOK_SETTING, data).then((result) => {
			if (result === true) {
				toast.success("Discord settings saved!", {
					position: toast.POSITION.TOP_RIGHT,
				});
			} else {
				toast.warning("Discord settings not saved! Try again later", {
					position: toast.POSITION.TOP_RIGHT,
				});
			}
		});
	};

	const onChecking = async () => {
		const url = getValues("url");
		await checkUrl(url, true);
	};

	const checkUrl = async (url, show) => {
		if (url === "") {
			toast.error("Webhook url is empty", {
				position: toast.POSITION.TOP_RIGHT,
			});
			setError("url", { message: "Input webhook url" });
			return;
		}
		const result = await checkUrlDiscordWebhook(url);
		if (result === true) {
			if (show) {
				toast.success("Webhook url is valid", {
					position: toast.POSITION.TOP_RIGHT,
				});
			}
		} else {
			toast.error("Webhook url is invaild", {
				position: toast.POSITION.TOP_RIGHT,
			});
			setError("url", { message: "Reinput webhook url" });
		}
	};

	return (
		<>
			<ContentBox>
				<Typography variant="h6">Discord Settings</Typography>
				<form onSubmit={handleSubmit(onSubmit)}>
					<Grid container spacing={3}>
						<Grid item xs={12}>
							<MUISwitch
								name="active"
								control={control}
								label="Active"
							/>
						</Grid>
						<Grid item xs={12}>
							<MUISwitch
								name="online"
								control={control}
								label="Send message when account online"
							/>
						</Grid>
						<Grid item xs={8}>
							<TextField
								fullWidth
								label="URL webhook"
								variant="outlined"
								error={!!errors.url}
								helperText={errors.url?.message}
								{...register("url")}
							/>
						</Grid>
						<Grid item xs={4}>
							<Button variant="contained" onClick={onChecking}>
								Check
							</Button>
						</Grid>
						<Grid item xs={12}>
							<Button
								type="submit"
								variant="contained"
								value="submit"
								color="success"
							>
								Save
							</Button>
						</Grid>
					</Grid>
				</form>
			</ContentBox>
		</>
	);
};
export default DiscordSettings;
