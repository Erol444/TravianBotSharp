import { Button, Grid, TextField, Typography } from "@mui/material";
import { useForm } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";

import ContentBox from "../../ContentBox";
import ActivitySchema from "../../../yup/Settings/ActivitySchema.js";

import React, { useEffect } from "react";
import { toast } from "react-toastify";
import { getSetting, setSetting, ACTIVITY_SETTING } from "../../../api/api";
import { useSelector } from "react-redux";

const ActivitySettings = () => {
	const {
		register,
		handleSubmit,
		formState: { errors },
		setValue,
	} = useForm({
		resolver: yupResolver(ActivitySchema),
	});

	const account = useSelector((state) => state.account.info.id);
	const onSubmit = (data) => {
		setSetting(account, ACTIVITY_SETTING, data).then((result) => {
			if (result === true) {
				toast.success("Activity settings saved!", {
					position: toast.POSITION.TOP_RIGHT,
				});
			} else {
				toast.warning("Activity settings not saved! Try again later", {
					position: toast.POSITION.TOP_RIGHT,
				});
			}
		});
	};

	useEffect(() => {
		if (account !== -1) {
			getSetting(account, ACTIVITY_SETTING).then((data) => {
				const { sleepTime, workTime } = data;
				setValue("sleep.min", sleepTime.min);
				setValue("sleep.max", sleepTime.max);
				setValue("work.min", workTime.min);
				setValue("work.max", workTime.max);
			});
		}
	}, [account, setValue]);

	return (
		<>
			<ContentBox>
				<Typography variant="h6">Acitvity Settings</Typography>
				<form onSubmit={handleSubmit(onSubmit)}>
					<Grid container spacing={3}>
						<Grid item xs={2}>
							<Typography variant="subtitle1">
								Working time:
							</Typography>
						</Grid>
						<Grid item xs={5}>
							<TextField
								fullWidth
								label="Min"
								variant="outlined"
								type="number"
								error={!!errors.work?.min}
								helperText={errors.work?.min?.message}
								{...register("work.min")}
							/>
						</Grid>
						<Grid item xs={5}>
							<TextField
								fullWidth
								label="Max"
								variant="outlined"
								type="number"
								error={!!errors.work?.max}
								helperText={errors.work?.max?.message}
								{...register("work.max")}
							/>
						</Grid>
						<Grid item xs={2}>
							<Typography variant="subtitle1">
								Sleeping time:
							</Typography>
						</Grid>
						<Grid item xs={5}>
							<TextField
								fullWidth
								label="Min"
								variant="outlined"
								type="number"
								error={!!errors.sleep?.min}
								helperText={errors.sleep?.min?.message}
								{...register("sleep.min")}
							/>
						</Grid>
						<Grid item xs={5}>
							<TextField
								fullWidth
								label="Max"
								variant="outlined"
								type="number"
								error={!!errors.sleep?.max}
								helperText={errors.sleep?.max?.message}
								{...register("sleep.max")}
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
export default ActivitySettings;
