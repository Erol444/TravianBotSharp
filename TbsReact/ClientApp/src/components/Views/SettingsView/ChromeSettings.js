import React, { useEffect } from "react";
import { useForm } from "react-hook-form";
import {
	List,
	ListItem,
	ListItemText,
	Grid,
	Typography,
	TextField,
	Button,
	Switch,
	Box,
} from "@mui/material";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { toast } from "react-toastify";

import { getChromeSetting, setChromeSetting } from "../../../api/Setting";
import { useSelector } from "react-redux";

import style from "../../../styles/box";

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
				const { workTime, sleepTime, disableImages, click, autoClose } =
					data;
				setValue("work_min", workTime.min);
				setValue("work_max", workTime.max);
				setValue("sleep_min", sleepTime.min);
				setValue("sleep_max", sleepTime.max);
				setValue("click_min", click.min);
				setValue("click_max", click.max);
				setValue("disable_image", disableImages);
				setValue("close_chrome", autoClose);
			});
		}
	}, [account, setValue]);

	return (
		<>
			<Box sx={style}>
				<Typography
					variant="h6"
					noWrap
					component="div"
					sx={{ mr: 2, display: { xs: "none", md: "flex" } }}
				>
					Chrome settings
				</Typography>
				<form onSubmit={handleSubmit(onSubmit)}>
					<Grid container spacing={1}>
						<Grid item xs={2}>
							<Typography
								variant="subtitle1"
								noWrap
								component="div"
								sx={{
									mr: 2,
									display: { xs: "none", md: "flex" },
								}}
							>
								Work time
							</Typography>
						</Grid>
						<Grid item xs={5}>
							<TextField
								id="outlined-number"
								label="Min"
								type="number"
								InputLabelProps={{
									shrink: true,
								}}
								error={!!errors.work_min}
								helperText={errors.work_min?.message}
								{...register("work_min")}
							/>
						</Grid>
						<Grid item xs={5}>
							<TextField
								id="outlined-number"
								label="Max"
								type="number"
								InputLabelProps={{
									shrink: true,
								}}
								error={!!errors.work_max}
								helperText={errors.work_max?.message}
								{...register("work_max")}
							/>
						</Grid>

						<Grid item xs={2}>
							<Typography
								variant="subtitle1"
								noWrap
								component="div"
								sx={{
									mr: 2,
									display: { xs: "none", md: "flex" },
								}}
							>
								Sleep time
							</Typography>
						</Grid>
						<Grid item xs={5}>
							<TextField
								id="outlined-number"
								label="Min"
								type="number"
								InputLabelProps={{
									shrink: true,
								}}
								error={!!errors.sleep_min}
								helperText={errors.sleep_min?.message}
								{...register("sleep_min")}
							/>
						</Grid>
						<Grid item xs={5}>
							<TextField
								id="outlined-number"
								label="Max"
								type="number"
								InputLabelProps={{
									shrink: true,
								}}
								error={!!errors.sleep_max}
								helperText={errors.sleep_max?.message}
								{...register("sleep_max")}
							/>
						</Grid>

						<Grid item xs={2}>
							<Typography
								variant="subtitle1"
								noWrap
								component="div"
								sx={{
									mr: 2,
									display: { xs: "none", md: "flex" },
								}}
							>
								Click delay
							</Typography>
						</Grid>
						<Grid item xs={5}>
							<TextField
								id="outlined-number"
								label="Min"
								type="number"
								InputLabelProps={{
									shrink: true,
								}}
								error={!!errors.click_min}
								helperText={errors.click_min?.message}
								{...register("click_min")}
							/>
						</Grid>
						<Grid item xs={5}>
							<TextField
								id="outlined-number"
								label="Max"
								type="number"
								InputLabelProps={{
									shrink: true,
								}}
								error={!!errors.click_max}
								helperText={errors.click_max?.message}
								{...register("click_max")}
							/>
						</Grid>
						<Grid item xs={3}>
							<List>
								<ListItem>
									<ListItemText primary="Disable image" />
									<Switch {...register("disable_image")} />
								</ListItem>
								<ListItem>
									<ListItemText primary="Close when no task in 5 mins" />
									<Switch {...register("close_chrome")} />
								</ListItem>
							</List>
						</Grid>
					</Grid>
					<Button type="submit" variant="contained" value="submit">
						Save
					</Button>
				</form>
			</Box>
		</>
	);
};

export default ChromeSettings;
