import React, { useEffect } from "react";
import { useForm } from "react-hook-form";
import {
	Grid,
	Typography,
	TextField,
	Button,
	Switch,
	FormControlLabel,
} from "@mui/material";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { toast } from "react-toastify";

import { getChromeSetting, setChromeSetting } from "../../../api/Setting";
import { useSelector } from "react-redux";

import ContentBox from "../../ContentBox";

const schema = yup
	.object()
	.shape({
		work_min: yup
			.number()
			.min(1)
			.integer()
			.label("Work time min")
			.typeError("Work time min must be a number"),
		work_max: yup
			.number()
			.moreThan(yup.ref("work_min"))
			.integer()
			.label("Work time max")
			.typeError("Work time max must be a number"),
		sleep_min: yup
			.number()
			.min(1)
			.integer()
			.label("Sleep time min")
			.typeError("Sleep time min must be a number"),
		sleep_max: yup
			.number()
			.moreThan(yup.ref("sleep_min"))
			.integer()
			.label("Sleep time max")
			.typeError("Sleep time max must be a number"),
		click_min: yup
			.number()
			.min(1)
			.integer()
			.label("Click time min")
			.typeError("Click time min must be a number"),
		click_max: yup
			.number()
			.moreThan(yup.ref("click_min"))
			.integer()
			.label("Click time max")
			.typeError("Click time max must be a number"),
	})
	.required();

const ChromeSettings = () => {
	const {
		register,
		handleSubmit,
		formState: { errors },
		setValue,
	} = useForm({
		resolver: yupResolver(schema),
	});
	const account = useSelector((state) => state.account.info.id);
	const onSubmit = async (data) => {
		await setChromeSetting(account, data);
		toast.success("Chrome settings saved !", {
			position: toast.POSITION.TOP_RIGHT,
		});
	};

	useEffect(() => {
		if (account !== -1) {
			getChromeSetting(account).then((data) => {
				const { disableImages, click, autoClose } = data;
				setValue("click.min", click.min);
				setValue("click.max", click.max);
				setValue("disable_image", disableImages);
				setValue("close_chrome", autoClose);
			});
		}
	}, [account, setValue]);

	return (
		<>
			<ContentBox>
				<Typography variant="h6">Chrome Settings</Typography>
				<form onSubmit={handleSubmit(onSubmit)}>
					<Grid container spacing={3}>
						<Grid item xs={2}>
							<Typography variant="subtitle1">
								Click delay:
							</Typography>
						</Grid>
						<Grid item xs={5}>
							<TextField
								fullWidth
								label="Min"
								variant="outlined"
								type="number"
								error={!!errors.click?.min}
								helperText={errors.click?.min?.message}
								{...register("click.min")}
							/>
						</Grid>
						<Grid item xs={5}>
							<TextField
								fullWidth
								label="Max"
								variant="outlined"
								type="number"
								error={!!errors.click?.max}
								helperText={errors.click?.max?.message}
								{...register("click.max")}
							/>
						</Grid>
						<Grid item xs={12}>
							<FormControlLabel
								control={
									<Switch {...register("disable_image")} />
								}
								label="Disable images"
							/>
						</Grid>
						<Grid item xs={12}>
							<FormControlLabel
								control={
									<Switch {...register("close_chrome")} />
								}
								label="Close if no task in 5 minutes"
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

export default ChromeSettings;
