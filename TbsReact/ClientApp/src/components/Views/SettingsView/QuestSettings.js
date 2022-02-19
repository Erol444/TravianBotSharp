import { Button, Grid, Typography } from "@mui/material";
import { useForm } from "react-hook-form";

import ContentBox from "../../ContentBox";
import MUISelect from "../../ref/MUISelect";
import MUISwitch from "../../ref/MUISwitch";
import React, { useState, useEffect } from "react";
import { toast } from "react-toastify";
import {
	getSetting,
	setSetting,
	QUEST_SETTING,
	getVillages,
} from "../../../api/api";
import { useSelector } from "react-redux";

const QuestSettings = () => {
	const { control, handleSubmit, setValue } = useForm();
	const account = useSelector((state) => state.account.info.id);
	const [villages, setVillages] = useState([]);

	const onSubmit = (data) => {
		setSetting(account, QUEST_SETTING, data).then((result) => {
			if (result === true) {
				toast.success("Quest settings saved!", {
					position: toast.POSITION.TOP_RIGHT,
				});
			} else {
				toast.warning("Quest settings not saved! Try again later", {
					position: toast.POSITION.TOP_RIGHT,
				});
			}
		});
	};

	useEffect(() => {
		if (account !== -1) {
			getSetting(account, QUEST_SETTING).then((data) => {
				const { beginner, daily, villageId } = data;
				setValue("beginner", beginner);
				setValue("daily", daily);

				getVillages(account).then((data) => {
					setVillages(data);
					setValue("villageId", villageId);
				});
			});
		}
	}, [account, setValue]);

	return (
		<>
			<ContentBox>
				<Typography variant="h6">Quest Settings</Typography>
				<form onSubmit={handleSubmit(onSubmit)}>
					<Grid container spacing={3}>
						<Grid item xs={12}>
							<MUISwitch
								name="daily"
								control={control}
								label="Daily quest claim"
							/>
						</Grid>
						<Grid item xs={12}>
							<MUISwitch
								name="beginner"
								control={control}
								label="Beginner quest claim"
							/>
						</Grid>
						<Grid item xs={12}>
							<MUISelect
								label="Village claim"
								name="villageId"
								control={control}
								options={villages}
								fullWidth
							/>
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
export default QuestSettings;
